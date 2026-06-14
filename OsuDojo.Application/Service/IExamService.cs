using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;

namespace OsuDojo.Application.Service;

public interface IExamService
{
    IExamTracker BuildExamTracker(string gameMode, ExamQuery examQuery, int[] playlistIds, int[] totalLengths);
}
