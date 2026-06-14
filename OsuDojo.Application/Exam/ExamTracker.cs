using System.Text.Json;
using OsuDojo.Application.Query;
using OsuDojo.Exam.Criteria;
using OsuDojo.Exam.Stage;

namespace OsuDojo.Application.Exam;

public class ExamTracker<T> : IExamTracker where T : IGradeCutoff
{
    private readonly ExamBeatmapStage<T>[] _beatmapStages;
    private readonly ExamStage<T> _finalStage = new();
    private int _currentStageIndex;

    public ExamTracker(ExamQuery examQuery, int[] playlistIds, int[] totalLengths)
    {
        var beatmapCount = examQuery.ExamContent.BeatmapContents.Length;
        _beatmapStages = new ExamBeatmapStage<T>[beatmapCount];

        for (var i = 0; i < beatmapCount; i++)
        {
            _beatmapStages[i] = new ExamBeatmapStage<T>(
                examQuery.ExamContent.BeatmapContents[i].BeatmapId,
                playlistIds[i],
                totalLengths[i]);

            _beatmapStages[i].SetUpCriteria(
                examQuery.ExamContent.BeatmapContents[i].GradeCutoffs.Select(x => x.Deserialize<T>()));
        }

        _finalStage.SetUpCriteria(examQuery.ExamContent.OverallGradeCutoffs.Select(x => x.Deserialize<T>()));
    }

    public int CurrentStage => Math.Min(_currentStageIndex + 1, _beatmapStages.Length);
    public int CurrentPlaylistId => !IsEnded ? _beatmapStages[_currentStageIndex].PlaylistId : 0;
    public int CurrentBeatmapId => !IsEnded ? _beatmapStages[_currentStageIndex].Id : 0;
    public int CurrentBeatmapLength => !IsEnded ? _beatmapStages[_currentStageIndex].Length : 0;
    public bool IsEnded => _currentStageIndex >= _beatmapStages.Length;
    public StageResult[] StageResults => _beatmapStages.Select(x => (StageResult)x.Result).ToArray();
    public int PassGrade { get; private set; }

    public bool IsModListValid(string[] mods)
    {
        // TODO: Might implement real mod list check if necessary
        return mods.Length == 0;
    }

    public bool TryAdvance(BeatmapResultQuery beatmapResultQuery)
    {
        if (IsEnded)
        {
            throw new InvalidOperationException("Exam has already ended.");
        }

        var beatmapPassGrade = _beatmapStages[_currentStageIndex].Evaluate(beatmapResultQuery.Result);
        PassGrade = _currentStageIndex == 0 ? beatmapPassGrade : Math.Min(PassGrade, beatmapPassGrade);

        if (PassGrade == 0)
        {
            return false;
        }

        _currentStageIndex++;

        if (!IsEnded)
        {
            return true;
        }

        var finalResult = StageResults.Aggregate(new StageResult(), (current, next) => current + next);
        PassGrade = Math.Min(PassGrade, _finalStage.Evaluate(finalResult));
        return PassGrade > 0;
    }
}
