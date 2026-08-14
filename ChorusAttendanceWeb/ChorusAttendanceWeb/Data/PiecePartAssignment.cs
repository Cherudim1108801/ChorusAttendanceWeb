using ChorusAttendanceWeb.Models;

namespace ChorusAttendanceWeb.Data;

/// <summary>曲におけるパートの割り振り（使用パートと内部分割方式）の1件分。</summary>
public class PiecePartAssignment
{
    public Guid Id { get; set; }

    public Guid PieceId { get; set; }
    public Piece? Piece { get; set; }

    public PartType Part { get; set; }
    public PartDivision Division { get; set; }

    /// <summary>内部分割を反映した、このパートの実際のパート名一覧。</summary>
    public IReadOnlyList<string> SubPartLabels => Division.ToSubPartLabels(Part);
}
