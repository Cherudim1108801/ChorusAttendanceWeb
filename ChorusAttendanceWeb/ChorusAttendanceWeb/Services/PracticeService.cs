using ChorusAttendanceWeb.Data;
using ChorusAttendanceWeb.Models;
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

    /// <summary>今日以降の練習予定を、日付の古い順に取得する。</summary>
    public async Task<List<Practice>> GetUpcomingAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await db.Practices
            .Include(p => p.Pieces).ThenInclude(pp => pp.Piece)
            .Where(p => p.Date >= today)
            .OrderBy(p => p.Date)
            .ToListAsync(ct);
    }

    /// <summary>今日より前の練習予定（過去の練習データ）を、日付の新しい順に取得する。</summary>
    public async Task<List<Practice>> GetPastAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await db.Practices
            .Include(p => p.Pieces).ThenInclude(pp => pp.Piece)
            .Where(p => p.Date < today)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);
    }

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

    /// <summary>「音源」タブに表示する、録音登録済みの曲を練習日の新しい順に横断的に取得する。</summary>
    public async Task<List<RecordingItem>> GetRecordingItemsAsync(CancellationToken ct = default)
    {
        var practices = await GetAllAsync(ct);
        return practices
            .OrderByDescending(p => p.Date)
            .SelectMany(p => p.Pieces
                .Where(pp => !string.IsNullOrEmpty(pp.RecordingUrl) && pp.Piece is not null)
                .Select(pp => new RecordingItem
                {
                    PracticeId = p.Id,
                    PieceId = pp.PieceId,
                    Title = pp.Piece!.Title,
                    PracticeLabel = string.IsNullOrEmpty(p.Title)
                        ? $"{p.Date:yyyy年M月d日}の練習"
                        : $"{p.Date:yyyy年M月d日} {p.Title}",
                    RecordingUrl = pp.RecordingUrl!,
                    IsFeatured = pp.IsFeatured
                }))
            .ToList();
    }

    /// <summary>新しい練習予定を登録する。</summary>
    /// <returns>発行された練習予定ID。</returns>
    public async Task<Guid> CreateAsync(DateOnly date, string title, string place, IReadOnlyList<Guid> pieceIds, bool requiresKeyPickup = false, CancellationToken ct = default)
    {
        var practice = new Practice
        {
            Id = Guid.NewGuid(),
            Date = date,
            Title = title,
            Place = place,
            CreatedAt = DateTime.UtcNow,
            RequiresKeyPickup = requiresKeyPickup,
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

    /// <summary>指定の練習の鍵の受け取り状況を設定する。</summary>
    public async Task SetKeyPickedUpAsync(Guid practiceId, bool keyPickedUp, CancellationToken ct = default)
    {
        var practice = await db.Practices.FirstOrDefaultAsync(p => p.Id == practiceId, ct);
        if (practice is null)
            return;

        practice.KeyPickedUp = keyPickedUp;
        await db.SaveChangesAsync(ct);
    }

    /// <summary>指定の練習における、指定の曲の録音音源リンクを設定・変更・削除する。</summary>
    /// <param name="recordingUrl">録音音源へのリンク（OneDriveなど）。削除する場合は null。</param>
    public async Task SetPieceRecordingUrlAsync(Guid practiceId, Guid pieceId, string? recordingUrl, CancellationToken ct = default)
    {
        var practicePiece = await db.PracticePieces
            .FirstOrDefaultAsync(pp => pp.PracticeId == practiceId && pp.PieceId == pieceId, ct);
        if (practicePiece is null)
            return;

        practicePiece.RecordingUrl = recordingUrl;
        await db.SaveChangesAsync(ct);
    }

    /// <summary>指定の練習における、指定の曲の録音を「音源」タブで強調表示（ピン留め）するかどうかを設定する。</summary>
    public async Task SetPieceRecordingFeaturedAsync(Guid practiceId, Guid pieceId, bool isFeatured, CancellationToken ct = default)
    {
        var practicePiece = await db.PracticePieces
            .FirstOrDefaultAsync(pp => pp.PracticeId == practiceId && pp.PieceId == pieceId, ct);
        if (practicePiece is null)
            return;

        practicePiece.IsFeatured = isFeatured;
        await db.SaveChangesAsync(ct);
    }
}
