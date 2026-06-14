using OsuDojo.Application.Context;
using OsuDojo.Application.Exam;
using OsuDojo.Application.Repository;
using OsuDojo.Application.Service;
using OsuDojo.Application.Utility;

namespace OsuDojo.Application.Worker;

public class BeatmapResultPollingWorker(
    IOsuMultiplayerRoomService osuMultiplayerRoomService,
    IExamSessionRepository examSessionRepository,
    IRankCertificateRepository rankCertificateRepository,
    IUserRepository userRepository,
    string accessToken,
    ExamSessionContext examSessionContext)
    : WorkerBase
{
    private readonly IOsuMultiplayerRoomService _osuMultiplayerRoomService = osuMultiplayerRoomService;
    private readonly IExamSessionRepository _examSessionRepository = examSessionRepository;
    private readonly IRankCertificateRepository _rankCertificateRepository = rankCertificateRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly string _accessToken = accessToken;
    private readonly ExamSessionContext _examSessionContext = examSessionContext;

    protected override async Task Execute()
    {
        try
        {
            var beatmapResultQuery = await _osuMultiplayerRoomService.GetBeatmapResultAsync(
                _examSessionContext.RoomId,
                _examSessionContext.ExamTracker.CurrentPlaylistId,
                _accessToken);

            if (beatmapResultQuery == null)
            {
                return;
            }

            if (!_examSessionContext.ExamTracker.IsModListValid(beatmapResultQuery.Mods))
            {
                await _examSessionRepository.DisqualifyAsync(_examSessionContext.ExamSessionId);
                _examSessionContext.Status = ExamSessionStatus.Disqualified;
            }
            else if (!_examSessionContext.ExamTracker.TryAdvance(beatmapResultQuery))
            {
                await _examSessionRepository.SetCompletedAsync(_examSessionContext.ExamSessionId);
                _examSessionContext.Status = ExamSessionStatus.Failed;
            }
            else if (_examSessionContext.ExamTracker.IsEnded)
            {
                await _examSessionRepository.SetCompletedAsync(_examSessionContext.ExamSessionId);
                _examSessionContext.Status = ExamSessionStatus.Passed;

                var rankCertificateId = await _rankCertificateRepository.CreateAsync(
                    _examSessionContext.ExamSessionId,
                    _examSessionContext.ExamTracker.PassGrade,
                    _examSessionContext.ExamTracker.StageResults);

                await _userRepository.UpdateUserBestCertificateAsync(
                    _examSessionContext.UserId,
                    _examSessionContext.GameMode,
                    _examSessionContext.Rank,
                    rankCertificateId);
            }
            else
            {
                await _examSessionRepository.ProceedToNextStageAsync(_examSessionContext.ExamSessionId);
                _examSessionContext.Status = ExamSessionStatus.Waiting;

                new PlaylistStatusPollingWorker(
                        _osuMultiplayerRoomService,
                        _examSessionRepository,
                        _rankCertificateRepository,
                        _userRepository,
                        _accessToken,
                        _examSessionContext)
                    .Run(AppDefaults.OsuPollingInterval, AppDefaults.OsuPollingDuration);
            }
        }
        catch (Exception)
        {
            await _examSessionRepository.TerminateAsync(_examSessionContext.ExamSessionId);
            _examSessionContext.Status = ExamSessionStatus.Terminated;
            throw;
        }

        Cancel();
    }

    protected override async Task OnCompleted()
    {
        if (_examSessionContext.Status != ExamSessionStatus.Playing)
        {
            return;
        }

        await _examSessionRepository.SetNoResponseAsync(_examSessionContext.ExamSessionId);
        _examSessionContext.Status = ExamSessionStatus.NoResponse;
    }
}
