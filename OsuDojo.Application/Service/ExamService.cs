using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;
using OsuDojo.Exam.Taiko.Criteria;

namespace OsuDojo.Application.Service;

public class ExamService : IExamService
{
    public IExamTracker BuildExamTracker(string gameMode, ExamQuery examQuery, int[] playlistIds, int[] totalLengths)
    {
        return gameMode switch
        {
            _ => new ExamTracker<TaikoGradeCutoff>(examQuery, playlistIds, totalLengths)
        };
    }
}
