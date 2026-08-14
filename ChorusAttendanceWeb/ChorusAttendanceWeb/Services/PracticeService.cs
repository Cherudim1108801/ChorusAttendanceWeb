using ChorusAttendanceWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Services;

/// <summary>練習予定（<see cref="Practice"/>）の取得・作成・削除を担う。</summary>
public class PracticeService(ApplicationDbContext db)
{
    /// <summary>登録されている全練習予定を日付昇順で取得する。</summary>
    public async Task<List<Practice>> GetAllAsync(CancellationToken ct = default) =>
        await db.Practices
            .Include(p => p.Pieces).ThenInclude(pp => pp.Piece)
            .OrderBy(p => p.Date)
            .ToListAsync(ct);

    /// <summary>指定IDの練習予定を1件取得する。存在しない場合は null。</summary>
    public async Task<Practice?> GetByIdAsync(Guid practiceId, CancellationToken ct = default) =>
        await db.Practices
            .Include(p => p.Pieces).ThenInclude(pp => pp.Piece).ThenInclude(piece => piece!.PartAssignments)
            .FirstOrDefaultAsync(p => p.Id == practiceId, ct);

    /// <summary>今日以降で最も近い練習予定を取得する。存在しない場合は null。</summary>
    public async Task<Practice?> GetNextUpcomingAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await db.Practices
            .Include(p => p.Pieces).ThenInclude(pp => pp.Piece)
            .Where(p => p.Date >= today)
            .OrderBy(p => p.Date)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>新しい練習予定を登録する。</summary>
    /// <returns>発行された練習予定ID。</returns>
    public async Task<Guid> CreateAsync(DateOnly date, string title, string place, IReadOnlyList<Guid> pieceIds, CancellationToken ct = default)
    {
        var practice = new Practice
        {
            Id = Guid.NewGuid(),
            Date = date,
            Title = title,
            Place = place,
            CreatedAt = DateTime.UtcNow,
            Pieces = pieceIds.Select(id => new PracticePiece { PieceId = id }).ToList()
        };
        db.Practices.Add(practice);
        await db.SaveChangesAsync(ct);
        return practice.Id;
    }

    /// <summary>指定IDの練習予定を削除する。</summary>
    public async Task DeleteAsync(Guid practiceId, CancellationToken ct = default)
    {
        var practice = await db.Practices.FirstOrDefaultAsync(p => p.Id == practiceId, ct);
        if (practice is null)
            return;

        db.Practices.Remove(practice);
        await db.SaveChangesAsync(ct);
    }
}
