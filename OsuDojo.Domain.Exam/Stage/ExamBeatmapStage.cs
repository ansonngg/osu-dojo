using OsuDojo.Domain.Exam.Criteria;

namespace OsuDojo.Domain.Exam.Stage;

public class ExamBeatmapStage<T>(int beatmapId, int playlistId, int length) : ExamStage<T> where T : ICriteriaTable
{
    public int Id { get; } = beatmapId;
    public int PlaylistId { get; } = playlistId;
    public int Length { get; } = length;
    public IStageResult? Result { get; private set; }

    public override int Evaluate(IStageResult stageResult)
    {
        Result = stageResult;
        return base.Evaluate(Result);
    }
}
