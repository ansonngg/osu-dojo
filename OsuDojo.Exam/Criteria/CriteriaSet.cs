using OsuDojo.Exam.Stage;

namespace OsuDojo.Exam.Criteria;

public class CriteriaSet
{
    private readonly Dictionary<Type, CriteriaBase> _criteriaMap = new();

    public void Add(CriteriaBase criteria)
    {
        _criteriaMap.Add(criteria.GetType(), criteria);
    }

    public bool IsPassed(IStageResult stageResult)
    {
        return _criteriaMap.Values.All(x => x.IsSatisfied(stageResult));
    }
}
