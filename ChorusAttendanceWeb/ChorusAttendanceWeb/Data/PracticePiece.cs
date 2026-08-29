namespace ChorusAttendanceWeb.Data;

/// <summary>練習予定と、その回で演奏予定の曲との中間テーブル。</summary>
public class PracticePiece
{
    public Guid PracticeId { get; set; }
    public Practice? Practice { get; set; }

    public Guid PieceId { get; set; }
    public Piece? Piece { get; set; }

    /// <summary>この練習でのこの曲の録音音源へのリンク（OneDriveなど）。未登録の場合は null。</summary>
    public string? RecordingUrl { get; set; }

    /// <summary>「音源」タブで強調表示（ピン留め）されているかどうか。</summary>
    public bool IsFeatured { get; set; }
}
