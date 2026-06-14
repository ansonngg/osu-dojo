using OsuDojo.Exam.Criteria;
using OsuDojo.Exam.Stage;

namespace OsuDojo.Exam.Taiko.Criteria;

public class OkCountCriteria(int threshold) : CriteriaBase(threshold)
{
    public override bool IsSatisfied(IStageResult stageResult)
    {
        return stageResult.Ok < Threshold;
    }
}
