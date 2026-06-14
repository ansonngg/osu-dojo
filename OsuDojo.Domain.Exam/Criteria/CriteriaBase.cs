using OsuDojo.Domain.Exam.Stage;

namespace OsuDojo.Domain.Exam.Criteria;

public abstract class CriteriaBase(int threshold)
{
    protected int Threshold { get; } = threshold;

    public abstract bool IsSatisfied(IStageResult stageResult);
}
