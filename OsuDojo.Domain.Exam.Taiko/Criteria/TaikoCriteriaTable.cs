using System.Text.Json.Serialization;
using OsuDojo.Domain.Exam.Criteria;

namespace OsuDojo.Domain.Exam.Taiko.Criteria;

public class TaikoCriteriaTable : ICriteriaTable
{
    [JsonPropertyName("great_count")]
    [Criteria(typeof(GreatCountCriteria))]
    public int? GreatCount { get; init; }

    [JsonPropertyName("ok_count")]
    [Criteria(typeof(OkCountCriteria))]
    public int? OkCount { get; init; }

    [JsonPropertyName("miss_count")]
    [Criteria(typeof(MissCountCriteria))]
    public int? MissCount { get; init; }

    [JsonPropertyName("large_bonus_count")]
    [Criteria(typeof(LargeBonusCountCriteria))]
    public int? LargeBonusCount { get; init; }

    [JsonPropertyName("hit_count")]
    [Criteria(typeof(HitCountCriteria))]
    public int? HitCount { get; init; }

    [JsonPropertyName("max_combo")]
    [Criteria(typeof(MaxComboCriteria))]
    public int? MaxCombo { get; init; }
}
