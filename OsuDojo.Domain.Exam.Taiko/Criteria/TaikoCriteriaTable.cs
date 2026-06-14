using OsuDojo.Domain.Exam.Criteria;

namespace OsuDojo.Domain.Exam.Taiko.Criteria;

public class TaikoCriteriaTable : ICriteriaTable
{
    [Criteria(typeof(GreatCountCriteria))]
    public int GreatCount { get; init; }

    [Criteria(typeof(OkCountCriteria))]
    public int OkCount { get; init; }

    [Criteria(typeof(MissCountCriteria))]
    public int MissCount { get; init; }

    [Criteria(typeof(LargeBonusCountCriteria))]
    public int LargeBonusCount { get; init; }

    [Criteria(typeof(HitCountCriteria))]
    public int HitCount { get; init; }

    [Criteria(typeof(MaxComboCriteria))]
    public int MaxCombo { get; init; }
}
