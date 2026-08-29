using ChorusAttendanceWeb.Data;
using ChorusAttendanceWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Services;

/// <summary>ある練習に対する、パート別集計・出欠状況・曲別参加者内訳をまとめて算出する。</summary>
public class AttendanceSummaryService(ApplicationDbContext db, MemberService memberService)
{
    /// <summary>指定練習に対する集計結果一式を算出する。</summary>
    public async Task<PracticeAttendanceSummary> BuildAsync(Practice practice, string currentUserId, CancellationToken ct = default)
    {
        var members = await memberService.GetAllAsync(ct);
        var attendances = await db.Attendances
            .Where(a => a.PracticeId == practice.Id)
            .ToListAsync(ct);

        var statusByUserId = attendances.ToDictionary(a => a.UserId, a => a.Status);
        var myStatus = statusByUserId.GetValueOrDefault(currentUserId, AttendanceStatus.Undecided);

        var attendingUserIds = attendances
            .Where(a => a.Status == AttendanceStatus.Attending)
            .Select(a => a.UserId)
            .ToHashSet();

        var attendingByPart = members
            .Where(m => attendingUserIds.Contains(m.Id))
            .GroupBy(m => m.Part!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var attendingNamesByPart = members
            .Where(m => attendingUserIds.Contains(m.Id))
            .GroupBy(m => m.Part!.Value)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<string>)g.Select(m => m.DisplayName ?? string.Empty)
                    .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
                    .ToList());

        var membersByPart = members.GroupBy(m => m.Part!.Value).ToDictionary(g => g.Key, g => g.Count());

        var partSummaries = PartTypeExtensions.All.Select(part => new PartSummary
        {
            Part = part,
            Label = part.ToDisplayName(),
            ColorHex = part.ToColorHex(),
            CardBackgroundColorHex = part.ToCardBackgroundColorHex(),
            AttendingCount = attendingByPart.GetValueOrDefault(part),
            MemberCount = membersByPart.GetValueOrDefault(part),
            AttendeeNames = attendingNamesByPart.GetValueOrDefault(part) ?? []
        }).ToList();

        var attendingMembers = members
            .Where(m => attendingUserIds.Contains(m.Id))
            .OrderBy(m => m.Part)
            .ThenBy(m => m.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var pieceParts = await db.MemberPieceParts
            .Where(m => attendingUserIds.Contains(m.UserId))
            .ToListAsync(ct);
        var piecePartsByUser = pieceParts
            .GroupBy(m => m.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var songParticipations = new List<SongParticipation>();
        foreach (var practicePiece in practice.Pieces)
        {
            var piece = practicePiece.Piece;
            if (piece is null)
                continue;

            var dots = new List<ParticipationDot>();
            PartType? previousPart = null;
            foreach (var member in attendingMembers)
            {
                var assignment = piece.PartAssignments.FirstOrDefault(a => a.Part == member.Part);
                string? subLabel = null;
                if (assignment is { Division: PartDivision.UpperLower })
                {
                    var subPart = piecePartsByUser.GetValueOrDefault(member.Id)
                        ?.FirstOrDefault(p => p.PieceId == piece.Id)?.SubPart;
                    var labels = assignment.SubPartLabels;
                    if (subPart == labels[0])
                        subLabel = "上";
                    else if (subPart == labels[1])
                        subLabel = "下";
                }

                dots.Add(new ParticipationDot
                {
                    ColorHex = member.Part!.Value.ToCardBackgroundColorHex(),
                    SubLabel = subLabel,
                    IsGroupStart = previousPart is not null && previousPart != member.Part
                });
                previousPart = member.Part;
            }

            songParticipations.Add(new SongParticipation
            {
                PieceId = piece.Id,
                Title = piece.Title,
                Dots = dots,
                RecordingUrl = practicePiece.RecordingUrl,
                IsFeatured = practicePiece.IsFeatured
            });
        }

        return new PracticeAttendanceSummary
        {
            PartSummaries = partSummaries,
            SongParticipations = songParticipations,
            TotalAttending = attendingByPart.Values.Sum(),
            TotalMembers = members.Count,
            TotalResponded = attendances.Count(a => a.Status != AttendanceStatus.Undecided),
            MyStatus = myStatus
        };
    }
}

/// <summary>ある練習に対する集計結果一式。</summary>
public class PracticeAttendanceSummary
{
    public required List<PartSummary> PartSummaries { get; init; }
    public required List<SongParticipation> SongParticipations { get; init; }
    public int TotalAttending { get; init; }
    public int TotalMembers { get; init; }
    public int TotalResponded { get; init; }
    public AttendanceStatus MyStatus { get; init; }
}
