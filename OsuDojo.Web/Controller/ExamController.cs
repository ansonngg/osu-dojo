using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OsuDojo.Application.Context;
using OsuDojo.Application.Exam;
using OsuDojo.Application.Query;
using OsuDojo.Application.Repository;
using OsuDojo.Application.Service;
using OsuDojo.Application.Utility;
using OsuDojo.Application.Worker;
using OsuDojo.Web.Request;
using OsuDojo.Web.Response;
using OsuDojo.Web.Utility;

namespace OsuDojo.Web.Controller;

[ApiController]
[Route("api/[controller]")]
public class ExamController(
    IOsuMultiplayerRoomService osuMultiplayerRoomService,
    IExamService examService,
    IExamRepository examRepository,
    IExamSessionRepository examSessionRepository,
    IRankCertificateRepository rankCertificateRepository,
    IUserRepository userRepository,
    IMemoryCache memoryCache,
    ILogger<ExamController> logger)
    : ControllerBase
{
    private static readonly TimeSpan ExamSessionExpiry = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan ExamSessionStatusCheckInterval = TimeSpan.FromSeconds(1);
    private readonly IOsuMultiplayerRoomService _osuMultiplayerRoomService = osuMultiplayerRoomService;
    private readonly IExamService _examService = examService;
    private readonly IExamRepository _examRepository = examRepository;
    private readonly IExamSessionRepository _examSessionRepository = examSessionRepository;
    private readonly IRankCertificateRepository _rankCertificateRepository = rankCertificateRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILogger<ExamController> _logger = logger;

    [HttpGet("{gameMode:regex(^(osu|taiko|catch|mania)$)}")]
    public async Task<IActionResult> GetExams(string gameMode)
    {
        var examQueries = await _examRepository.GetAsync(gameMode);
        return Ok(examQueries.Select(_ConstructExamResponse).ToArray());
    }

    [HttpGet("{gameMode:regex(^(osu|taiko|catch|mania)$)}/{rank:int}")]
    public async Task<IActionResult> GetExam(string gameMode, int rank)
    {
        var examQuery = await _examRepository.GetAsync(gameMode, rank);

        if (examQuery == null)
        {
            return NotFound();
        }

        return Ok(_ConstructExamResponse(examQuery));
    }

    [Authorize(Roles = "Admin,Setter")]
    [HttpPost]
    public async Task<IActionResult> CreateExam([FromBody] CreateExamRequest request)
    {
        if (request.GameMode is not ("osu" or "taiko" or "catch" or "mania"))
        {
            return BadRequest();
        }

        await _examRepository.CreateAsync(request.GameMode, request.Rank, request.ExamContent, request.RequiredRank);
        return Ok();
    }

    [Authorize]
    [HttpPost("{gameMode:regex(^(osu|taiko|catch|mania)$)}/{rank:int}")]
    public async Task<IActionResult> StartExamSession(string gameMode, int rank)
    {
        var examQuery = await _examRepository.GetAsync(gameMode, rank);

        if (examQuery == null)
        {
            return NotFound();
        }

        var accessToken = User.FindFirstValue(CustomClaimTypes.AccessToken);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            || !int.TryParse(User.FindFirstValue(CustomClaimTypes.OsuId), out var osuId)
            || string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized();
        }

        var inProgressExamSessionId = await _examSessionRepository.GetInProgressIdAsync(userId);

        if (inProgressExamSessionId != null)
        {
            return Conflict(new ExamSessionResponse { Id = inProgressExamSessionId.Value });
        }

        if ((await _userRepository.GetRankAsync(userId, gameMode)).Rank < examQuery.RequiredRank)
        {
            return BadRequest("User does not meet the lowest rank requirement.");
        }

        var multiplayerRoomQuery = await _osuMultiplayerRoomService.GetMostRecentActiveRoomAsync(osuId);

        if (multiplayerRoomQuery is not { IsActive: true } || multiplayerRoomQuery.Status != "idle")
        {
            return BadRequest(new ExamSessionResponse { IsRoomActive = false });
        }

        var beatmapCount = examQuery.ExamContent.BeatmapContents.Length;

        if (multiplayerRoomQuery.ActivePlaylistCount != beatmapCount)
        {
            return BadRequest(new ExamSessionResponse { IsRoomActive = true });
        }

        var roomPlaylistQuery = await _osuMultiplayerRoomService.GetRoomPlaylistAsync(multiplayerRoomQuery.RoomId);
        var isPlaylistCorrect = new bool[beatmapCount];
        var isExamValid = true;

        for (var i = 0; i < beatmapCount; i++)
        {
            if (roomPlaylistQuery.BeatmapIds[^(beatmapCount - i)] == examQuery.ExamContent.BeatmapContents[i].BeatmapId)
            {
                isPlaylistCorrect[i] = true;
            }
            else
            {
                isPlaylistCorrect[i] = false;
                isExamValid = false;
            }
        }

        if (!isExamValid)
        {
            return BadRequest(new ExamSessionResponse { IsRoomActive = true, IsPlaylistCorrect = isPlaylistCorrect });
        }

        var examSessionId = await _examSessionRepository.CreateAsync(userId, examQuery.ExamId);

        var examSessionContext = new ExamSessionContext
        {
            ExamSessionId = examSessionId,
            UserId = userId,
            OsuId = osuId,
            GameMode = gameMode,
            Rank = rank,
            RoomId = multiplayerRoomQuery.RoomId,
            ExamTracker = _examService.BuildExamTracker(
                gameMode,
                examQuery,
                roomPlaylistQuery.PlaylistIds[^beatmapCount..],
                roomPlaylistQuery.TotalLengths[^beatmapCount..])
        };

        _memoryCache.SetTyped(examSessionId, examSessionContext, ExamSessionExpiry);

        new PlaylistStatusPollingWorker(
                _osuMultiplayerRoomService,
                _examSessionRepository,
                _rankCertificateRepository,
                _userRepository,
                accessToken,
                examSessionContext)
            .Run(AppDefaults.OsuPollingInterval, AppDefaults.OsuPollingDuration);

        return Ok(
            new ExamSessionResponse
            {
                Id = examSessionId,
                IsRoomActive = true,
                IsPlaylistCorrect = isPlaylistCorrect
            });
    }

    [Authorize]
    [HttpGet("session/{examSessionId:int}")]
    public async Task GetExamSessionEvent(int examSessionId)
    {
        if (!_memoryCache.TryGetTyped<ExamSessionContext>(examSessionId, out var examSessionContext))
        {
            Response.StatusCode = 404;
            return;
        }

        if (examSessionContext == null)
        {
            throw new NullReferenceException("Exam session context is null.");
        }

        var accessToken = User.FindFirstValue(CustomClaimTypes.AccessToken);

        if (string.IsNullOrEmpty(accessToken))
        {
            Response.StatusCode = 401;
            return;
        }

        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        await _SendExamSessionStreamAsync(examSessionId, examSessionContext);

        var status = examSessionContext.Status;
        var timer = new PeriodicTimer(ExamSessionStatusCheckInterval);

        try
        {
            while (await timer.WaitForNextTickAsync()
                   && examSessionContext.Status is ExamSessionStatus.Waiting or ExamSessionStatus.Playing)
            {
                HttpContext.RequestAborted.ThrowIfCancellationRequested();

                if (examSessionContext.Status == status)
                {
                    continue;
                }

                await _SendExamSessionStreamAsync(examSessionId, examSessionContext);
                status = examSessionContext.Status;
            }

            await _SendExamSessionStreamAsync(examSessionId, examSessionContext);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Client disconnected from exam session {ExamSessionId}.", examSessionId);
        }
    }

    private static ExamResponse _ConstructExamResponse(ExamQuery examQuery)
    {
        return new ExamResponse { Rank = examQuery.Rank, ExamContent = examQuery.ExamContent };
    }

    private async Task _SendExamSessionStreamAsync(int examSessionId, ExamSessionContext examSessionContext)
    {
        int? maxWaitingTime = examSessionContext.Status switch
        {
            ExamSessionStatus.Waiting => AppDefaults.OsuPollingDuration.Seconds,
            ExamSessionStatus.Playing => examSessionContext.ExamTracker.CurrentBeatmapLength
                + AppDefaults.OsuPollingDuration.Seconds,
            _ => null
        };

        var payload = JsonSerializer.Serialize(
            new
            {
                osu_id = examSessionContext.OsuId,
                game_mode = examSessionContext.GameMode,
                rank = examSessionContext.Rank,
                stage = examSessionContext.ExamTracker.CurrentStage,
                stage_results = examSessionContext.ExamTracker.StageResults,
                status = examSessionContext.Status.ToSnakeCase(),
                timeout_at = maxWaitingTime != null
                    ? (DateTime?)examSessionContext.LastUpdatedAt + TimeSpan.FromSeconds(maxWaitingTime.Value)
                    : null,
                pass_grade = examSessionContext.Status == ExamSessionStatus.Passed
                    ? (int?)examSessionContext.ExamTracker.PassGrade
                    : null
            });

        await Response.WriteAsync($"id: {examSessionId}\n");
        await Response.WriteAsync($"data: {payload}\n\n");
        await Response.Body.FlushAsync();
    }
}
