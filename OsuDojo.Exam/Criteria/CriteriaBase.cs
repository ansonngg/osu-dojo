using OsuDojo.Exam.Stage;

namespace OsuDojo.Exam.Criteria;

public abstract class CriteriaBase(int threshold)
{
    protected int Threshold { get; } = threshold;

    public abstract bool IsSatisfied(IStageResult stageResult);
}
