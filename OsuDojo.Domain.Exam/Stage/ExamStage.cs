using System.Reflection;
using OsuDojo.Domain.Exam.Criteria;

namespace OsuDojo.Domain.Exam.Stage;

public class ExamStage<T> where T : ICriteriaTable
{
    private readonly List<CriteriaSet> _criteriaSets = [];

    public virtual int Evaluate(IStageResult stageResult)
    {
        for (var i = 0; i < _criteriaSets.Count; i++)
        {
            if (_criteriaSets[i].IsPassed(stageResult))
            {
                return _criteriaSets.Count - i;
            }
        }

        return 0;
    }

    public void SetUpCriteria(IEnumerable<T?> gradeCutoffs)
    {
        var propertyCriteriaTypePair = typeof(T)
            .GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(CriteriaAttribute), true).Length > 0)
            .Select(x => (x, x.GetCustomAttribute<CriteriaAttribute>(true)!.CriteriaType))
            .ToArray();

        foreach (var gradeCutoff in gradeCutoffs)
        {
            var criteriaSet = new CriteriaSet();

            foreach (var (property, criteriaType) in propertyCriteriaTypePair)
            {
                var propertyValue = property.GetValue(gradeCutoff);

                if (propertyValue == null)
                {
                    continue;
                }

                var rawCriteria = Activator.CreateInstance(criteriaType, propertyValue);

                if (rawCriteria is not CriteriaBase criteria)
                {
                    continue;
                }

                criteriaSet.Add(criteria);
            }

            _criteriaSets.Add(criteriaSet);
        }
    }
}
