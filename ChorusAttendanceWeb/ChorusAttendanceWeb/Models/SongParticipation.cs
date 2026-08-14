namespace ChorusAttendanceWeb.Models;

/// <summary>練習詳細画面における、曲ごとの参加者内訳（○表示用、非永続）。</summary>
public class SongParticipation
{
    public required Guid PieceId { get; init; }
    public required string Title { get; init; }
    public required List<ParticipationDot> Dots { get; init; }
}

/// <summary>参加者1人分を表す○1個分の表示データ。</summary>
public class ParticipationDot
{
    /// <summary>参加者が所属するパートの色（参加人数カードの塗りつぶし色と同一）。</summary>
    public required string ColorHex { get; init; }

    /// <summary>曲内で上下分割されている場合の「上」／「下」表示。分割なし・未選択の場合は null。</summary>
    public string? SubLabel { get; init; }

    /// <summary>パートの区切りを示すための強調表示（パートが切り替わる先頭の○のみ）。</summary>
    public bool IsGroupStart { get; init; }
}
