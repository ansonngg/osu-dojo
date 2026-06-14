using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OsuDojo.Application.Context;
using OsuDojo.Application.Options;
using OsuDojo.Application.Repository;
using OsuDojo.Application.Service;
using OsuDojo.Web.Request;
using OsuDojo.Web.Response;
using OsuDojo.Web.Utility;

namespace OsuDojo.Web.Controller;

[ApiController]
[Route("api/[controller]")]
public class OAuthController(
    IOsuAuthService osuAuthService,
    ILoginService loginService,
    IUserRepository userRepository,
    IOptions<LoginSessionOptions> loginSessionOptions,
    ILogger<OAuthController> logger)
    : ControllerBase
{
    private readonly IOsuAuthService _osuAuthService = osuAuthService;
    private readonly ILoginService _loginService = loginService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly int _cookieSessionExpiryInDay = loginSessionOptions.Value.CookieExpiryInDay;
    private readonly ILogger<OAuthController> _logger = logger;

    [HttpGet("url")]
    public IActionResult GetAuthorizeUrl()
    {
        var url = _osuAuthService.GetAuthorizeUrl();
        return Ok(new AuthorizeUrlResponse { Url = url });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tokenQuery = await _osuAuthService.ExchangeTokenAsync(request.Code);
        var osuId = await _osuAuthService.GetUserIdAsync(tokenQuery.AccessToken);
        var userRoleQuery = await _userRepository.GetRoleAsync(osuId) ?? await _userRepository.CreateAsync(osuId);

        var loginSessionContext = new LoginSessionContext
        {
            UserId = userRoleQuery.UserId,
            OsuId = osuId,
            Roles = userRoleQuery.Roles.Select(x => x.ToPascalCase()).ToArray(),
            AccessToken = tokenQuery.AccessToken,
            RefreshToken = tokenQuery.RefreshToken,
            ExpiresAt = tokenQuery.ExpiresAt
        };

        var sessionId = await _loginService.CreateLoginSessionAsync(loginSessionContext);

        Response.Cookies.Append(
            AuthenticationSettings.SessionCookieName,
            sessionId,
            DateTimeOffset.UtcNow.AddDays(_cookieSessionExpiryInDay));

        _logger.LogInformation("User with Id {OsuId} logged in.", osuId);
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var sessionId = Request.Cookies[AuthenticationSettings.SessionCookieName];

        if (string.IsNullOrEmpty(sessionId))
        {
            return BadRequest();
        }

        await _loginService.DeleteLoginSessionAsync(sessionId);
        Response.Cookies.Delete(AuthenticationSettings.SessionCookieName);
        return Ok();
    }
}
