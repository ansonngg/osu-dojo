using OsuDojo.Exam.Criteria;

namespace OsuDojo.Exam.Stage;

public class ExamBeatmapStage<T>(int beatmapId, int playlistId, int length) : ExamStage<T> where T : IGradeCutoff
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
