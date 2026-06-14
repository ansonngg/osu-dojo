using OsuDojo.Application.Exam;

namespace OsuDojo.Application.Repository;

public interface IRankCertificateRepository
{
    Task<int> CreateAsync(int examSessionId, int passGrade, StageResult[] stageResults);
}
