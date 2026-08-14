namespace ChorusAttendanceWeb.Data;

/// <summary>練習予定と、その回で演奏予定の曲との中間テーブル。</summary>
public class PracticePiece
{
    public Guid PracticeId { get; set; }
    public Practice? Practice { get; set; }

    public Guid PieceId { get; set; }
    public Piece? Piece { get; set; }
}
