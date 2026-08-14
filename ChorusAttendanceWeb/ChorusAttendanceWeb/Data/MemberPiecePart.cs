namespace ChorusAttendanceWeb.Data;

/// <summary>メンバーの、特定の曲における内部パート（分割）担当。</summary>
public class MemberPiecePart
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid PieceId { get; set; }
    public Piece? Piece { get; set; }

    public required string SubPart { get; set; }
}
