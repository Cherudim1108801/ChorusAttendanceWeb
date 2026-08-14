namespace ChorusAttendanceWeb.Data;

/// <summary>1回分の練習予定。</summary>
public class Practice
{
    public Guid Id { get; set; }

    /// <summary>練習日。時刻情報は持たないカレンダー日付。</summary>
    public DateOnly Date { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Place { get; set; } = string.Empty;

    public List<PracticePiece> Pieces { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    /// <summary>一覧表示用：演奏予定曲の曲名を読点区切りで結合した文字列。未選択の場合は空文字列。</summary>
    public string PiecesSummary => string.Join("、", Pieces.Select(p => p.Piece?.Title ?? string.Empty).Where(t => t.Length > 0));
}
