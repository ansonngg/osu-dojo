using System.Text.Json.Serialization;
using OsuDojo.Domain.Exam.Stage;

namespace OsuDojo.Application.Exam;

public class StageResult : IStageResult
{
    [JsonPropertyName("great")]
    public int Great { get; init; }

    [JsonPropertyName("ok")]
    public int Ok { get; init; }

    [JsonPropertyName("miss")]
    public int Miss { get; init; }

    [JsonPropertyName("large_bonus")]
    public int LargeBonus { get; init; }

    [JsonPropertyName("small_bonus")]
    public int SmallBonus { get; init; }

    [JsonPropertyName("max_combo")]
    public int MaxCombo { get; init; }

    public static StageResult operator +(StageResult left, StageResult right) =>
        new()
        {
            Great = left.Great + right.Great,
            Ok = left.Ok + right.Ok,
            Miss = left.Miss + right.Miss,
            LargeBonus = left.LargeBonus + right.LargeBonus,
            SmallBonus = left.SmallBonus + right.SmallBonus,
            MaxCombo = left.MaxCombo + right.MaxCombo
        };
}
