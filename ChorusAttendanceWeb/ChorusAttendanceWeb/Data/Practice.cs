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

    /// <summary>鍵の受け取りが必要かどうか。練習予定の登録時に選択する。管理者のみが参照できる情報。</summary>
    public bool RequiresKeyPickup { get; set; }

    /// <summary>鍵を受け取り済みかどうか。<see cref="RequiresKeyPickup"/> が true の場合のみ意味を持つ。管理者のみが参照・更新できる情報。</summary>
    public bool KeyPickedUp { get; set; }

    /// <summary>一覧表示用：演奏予定曲の曲名を読点区切りで結合した文字列。未選択の場合は空文字列。</summary>
    public string PiecesSummary => string.Join("、", Pieces.Select(p => p.Piece?.Title ?? string.Empty).Where(t => t.Length > 0));
}
