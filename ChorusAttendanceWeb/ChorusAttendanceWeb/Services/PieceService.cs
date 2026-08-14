using ChorusAttendanceWeb.Data;
using ChorusAttendanceWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChorusAttendanceWeb.Services;

/// <summary>合唱団のレパートリー曲（<see cref="Piece"/>）の取得・作成・削除を担う。</summary>
public class PieceService(ApplicationDbContext db)
{
    /// <summary>登録されている全曲を曲名順で取得する。</summary>
    public async Task<List<Piece>> GetAllAsync(CancellationToken ct = default) =>
        await db.Pieces
            .Include(p => p.PartAssignments)
            .OrderBy(p => p.Title)
            .ToListAsync(ct);

    /// <summary>指定IDの曲を1件取得する。存在しない場合は null。</summary>
    public async Task<Piece?> GetByIdAsync(Guid pieceId, CancellationToken ct = default) =>
        await db.Pieces
            .Include(p => p.PartAssignments)
            .FirstOrDefaultAsync(p => p.Id == pieceId, ct);

    /// <summary>新しい曲を登録する。</summary>
    /// <returns>発行された曲ID。</returns>
    public async Task<Guid> CreateAsync(string title, IReadOnlyList<(PartType Part, PartDivision Division)> partAssignments, CancellationToken ct = default)
    {
        var piece = new Piece
        {
            Id = Guid.NewGuid(),
            Title = title,
            CreatedAt = DateTime.UtcNow,
            PartAssignments = partAssignments
                .Select(a => new PiecePartAssignment { Id = Guid.NewGuid(), Part = a.Part, Division = a.Division })
                .ToList()
        };
        db.Pieces.Add(piece);
        await db.SaveChangesAsync(ct);
        return piece.Id;
    }

    /// <summary>指定IDの曲を削除する。</summary>
    public async Task DeleteAsync(Guid pieceId, CancellationToken ct = default)
    {
        var piece = await db.Pieces.FirstOrDefaultAsync(p => p.Id == pieceId, ct);
        if (piece is null)
            return;

        db.Pieces.Remove(piece);
        await db.SaveChangesAsync(ct);
    }
}
