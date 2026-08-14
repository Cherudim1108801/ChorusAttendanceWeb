using ChorusAttendanceWeb.Models;

namespace ChorusAttendanceWeb.Data;

/// <summary>合唱団のレパートリー曲1件。</summary>
public class Piece
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public List<PiecePartAssignment> PartAssignments { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    /// <summary>一覧表示用：割り振り済みパートを読点区切りで結合した文字列。</summary>
    public string PartsSummary => PartAssignments.Count == 0
        ? "パート未設定"
        : string.Join("、", PartAssignments.Select(a => a.Division == PartDivision.None
            ? a.Part.ToDisplayName()
            : $"{a.Part.ToDisplayName()}（{a.Division.ToDisplayName()}）"));
}
