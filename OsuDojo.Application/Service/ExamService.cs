using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;
using OsuDojo.Domain.Exam.Taiko.Criteria;

namespace OsuDojo.Application.Service;

public class ExamService : IExamService
{
    public IExamTracker BuildExamTracker(string gameMode, ExamQuery examQuery, int[] playlistIds, int[] totalLengths)
    {
        return gameMode switch
        {
            "osu" => throw new NotImplementedException(),
            "taiko" => new ExamTracker<TaikoCriteriaTable>(examQuery, playlistIds, totalLengths),
            "catch" => throw new NotImplementedException(),
            "mania" => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }
}
