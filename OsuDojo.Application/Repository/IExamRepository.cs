using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;

namespace OsuDojo.Application.Repository;

public interface IExamRepository
{
    public Task<ExamQuery[]> GetAsync(string gameMode);
    public Task<ExamQuery?> GetAsync(string gameMode, int rank);
    public Task CreateAsync(string gameMode, int rank, ExamContent examContent, int? requiredRank);
}
