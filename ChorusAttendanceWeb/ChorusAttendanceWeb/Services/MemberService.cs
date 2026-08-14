using ChorusAttendanceWeb.Data;
using ChorusAttendanceWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Services;

/// <summary>メンバー（<see cref="ApplicationUser"/>）のプロフィール情報の取得・保存を担う。</summary>
public class MemberService(ApplicationDbContext db)
{
    /// <summary>プロフィール登録済みの全メンバーを、パート → 氏名の順で取得する。</summary>
    public async Task<List<ApplicationUser>> GetAllAsync(CancellationToken ct = default) =>
        await db.Users
            .Where(u => u.DisplayName != null && u.Part != null)
            .OrderBy(u => u.Part)
            .ThenBy(u => u.DisplayName)
            .ToListAsync(ct);

    /// <summary>指定ユーザーの、曲ごとの内部パート担当を取得する。</summary>
    public async Task<List<MemberPiecePart>> GetPiecePartsAsync(string userId, CancellationToken ct = default) =>
        await db.MemberPieceParts.Where(m => m.UserId == userId).ToListAsync(ct);

    /// <summary>メンバーの名前・パートを新規登録または更新する（曲ごとの内部パート担当は変更しない）。</summary>
    public async Task SaveProfileAsync(ApplicationUser user, string name, PartType part, CancellationToken ct = default)
    {
        user.DisplayName = name;
        user.Part = part;
        user.ProfileUpdatedAt = DateTime.UtcNow;
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>メンバーの曲ごとの内部パート担当を一括で置き換える。</summary>
    public async Task SavePiecePartsAsync(string userId, IReadOnlyList<(Guid PieceId, string SubPart)> pieceParts, CancellationToken ct = default)
    {
        var existing = await db.MemberPieceParts.Where(m => m.UserId == userId).ToListAsync(ct);
        db.MemberPieceParts.RemoveRange(existing);

        foreach (var (pieceId, subPart) in pieceParts)
        {
            db.MemberPieceParts.Add(new MemberPiecePart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PieceId = pieceId,
                SubPart = subPart
            });
        }

        await db.SaveChangesAsync(ct);
    }
}
