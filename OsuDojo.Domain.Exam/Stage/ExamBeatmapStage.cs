using OsuDojo.Domain.Exam.Criteria;

namespace OsuDojo.Domain.Exam.Stage;

public class ExamBeatmapStage<T>(int beatmapId, int playlistId, int length) : ExamStage<T> where T : ICriteriaTable
{
    private IStageResult? _result;

    public int Id { get; } = beatmapId;
    public int PlaylistId { get; } = playlistId;
    public int Length { get; } = length;
    public IStageResult Result => _result ?? throw new InvalidOperationException("Result has not been set.");

    public override int Evaluate(IStageResult stageResult)
    {
        _result = stageResult;
        return base.Evaluate(Result);
    }
}
