using OsuDojo.Application.Query;

namespace OsuDojo.Application.Exam;

public interface IExamTracker
{
    int CurrentStage { get; }
    int CurrentPlaylistId { get; }
    int CurrentBeatmapId { get; }
    int CurrentBeatmapLength { get; }
    bool IsEnded { get; }
    StageResult[] StageResults { get; }
    int PassGrade { get; }

    bool IsModListValid(string[] mods);
    bool TryAdvance(BeatmapResultQuery beatmapResultQuery);
}
