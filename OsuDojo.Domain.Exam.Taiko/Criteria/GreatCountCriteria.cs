using OsuDojo.Domain.Exam.Criteria;
using OsuDojo.Domain.Exam.Stage;

namespace OsuDojo.Domain.Exam.Taiko.Criteria;

public class GreatCountCriteria(int threshold) : CriteriaBase(threshold)
{
    public override bool IsSatisfied(IStageResult stageResult)
    {
        return stageResult.Great >= Threshold;
    }
}
