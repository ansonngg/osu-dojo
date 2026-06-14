using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;
using OsuDojo.Application.Repository;
using OsuDojo.Application.Utility;
using OsuDojo.Infrastructure.Model;
using Supabase.Postgrest;

namespace OsuDojo.Infrastructure.Repository;

public class ExamRepository(Supabase.Client database, IMemoryCache memoryCache, ILogger<ExamRepository> logger)
    : IExamRepository
{
    private readonly Supabase.Client _database = database;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILogger<ExamRepository> _logger = logger;

    public async Task<ExamQuery[]> GetAsync(string gameMode)
    {
        var response = await _database.From<Exam>().Order(x => x.Rank, Constants.Ordering.Ascending).Get();

        return response != null
            ? response.Models
                .Select(
                    x =>
                    {
                        var examQuery = _ConstructExamQuery(x);

                        if (examQuery == null)
                        {
                            return new ExamQuery();
                        }

                        _memoryCache.SetTyped(x.Rank, examQuery);
                        return examQuery;
                    })
                .Where(x => x.ExamContent.BeatmapContents.Length > 0)
                .ToArray()
            : throw new NullReferenceException("Exam response is null.");
    }

    public async Task<ExamQuery?> GetAsync(string gameMode, int rank)
    {
        if (_memoryCache.TryGetTyped(rank, out ExamQuery? examQuery))
        {
            return examQuery;
        }

        var response = await _database.From<Exam>().Where(x => x.Rank == rank).Single();

        if (response == null)
        {
            return null;
        }

        examQuery = _ConstructExamQuery(response);

        if (examQuery != null)
        {
            _memoryCache.SetTyped(rank, examQuery);
        }

        return examQuery;
    }

    public async Task CreateAsync(string gameMode, int rank, ExamContent examContent, int? requiredRank)
    {
        await _database
            .From<Exam>()
            .Insert(
                new Exam
                {
                    GameMode = gameMode,
                    Rank = rank,
                    ExamContent = JsonSerializer.Serialize(examContent),
                    RequiredRank = requiredRank
                });

        _memoryCache.RemoveTyped<ExamQuery>(rank);
    }

    private ExamQuery? _ConstructExamQuery(Exam exam)
    {
        var examContent = JsonSerializer.Deserialize<ExamContent>(exam.ExamContent);

        if (examContent != null)
        {
            return new ExamQuery
            {
                ExamId = exam.Id,
                Rank = exam.Rank,
                ExamContent = examContent,
                RequiredRank = exam.RequiredRank
            };
        }

        _logger.LogError("Failed to deserialize exam content with id {ExamId}.", exam.Id);
        return null;
    }
}
