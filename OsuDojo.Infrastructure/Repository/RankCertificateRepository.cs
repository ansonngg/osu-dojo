using System.Text.Json;
using OsuDojo.Application.Exam;
using OsuDojo.Application.Repository;
using OsuDojo.Infrastructure.Model;
using Supabase.Postgrest;

namespace OsuDojo.Infrastructure.Repository;

public class RankCertificateRepository(Supabase.Client database) : IRankCertificateRepository
{
    private readonly Supabase.Client _database = database;

    public async Task<int> CreateAsync(int userId, int examSessionId, int passGrade, StageResult[] stageResults)
    {
        var response = await _database
            .From<RankCertificate>()
            .Insert(
                new RankCertificate
                {
                    UserId = userId,
                    ExamSessionId = examSessionId,
                    PassGrade = passGrade,
                    DetailedResult = JsonSerializer.Serialize(stageResults)
                },
                new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

        return response.Model?.Id ?? throw new NullReferenceException("Returned rank certificate is null.");
    }
}
