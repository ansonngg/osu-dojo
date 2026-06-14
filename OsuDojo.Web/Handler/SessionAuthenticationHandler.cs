using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using OsuDojo.Application.Context;
using OsuDojo.Application.Options;
using OsuDojo.Application.Repository;
using OsuDojo.Application.Service;
using OsuDojo.Web.Utility;

namespace OsuDojo.Web.Handler;

public class SessionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOsuAuthService osuAuthService,
    ILoginService loginService,
    IUserRepository userRepository,
    IOptions<LoginSessionOptions> loginSessionOptions,
    IOptions<OsuOptions> osuOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IOsuAuthService _osuAuthService = osuAuthService;
    private readonly ILoginService _loginService = loginService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly int _cookieSessionExpiryInDay = loginSessionOptions.Value.CookieExpiryInDay;
    private readonly int _tokenExpiryBufferInMinute = osuOptions.Value.TokenExpiryBufferInMinute;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var sessionId = Request.Cookies[AuthenticationSettings.SessionCookieName];

        if (string.IsNullOrEmpty(sessionId))
        {
            return AuthenticateResult.NoResult();
        }

        var loginSessionContext = await _loginService.GetLoginSessionAsync(sessionId);

        if (loginSessionContext == null)
        {
            Response.Cookies.Delete(AuthenticationSettings.SessionCookieName);
            return AuthenticateResult.NoResult();
        }

        if (DateTime.UtcNow >= loginSessionContext.ExpiresAt.AddMinutes(-_tokenExpiryBufferInMinute))
        {
            var newTokenQuery = await _osuAuthService.RefreshTokenAsync(loginSessionContext.RefreshToken);

            if (newTokenQuery == null)
            {
                await _loginService.DeleteLoginSessionAsync(sessionId);
                Response.Cookies.Delete(AuthenticationSettings.SessionCookieName);
                return AuthenticateResult.NoResult();
            }

            var roles = (await _userRepository.GetRoleAsync(loginSessionContext.OsuId))
                ?.Roles
                .Select(x => x.ToPascalCase())
                .ToArray();

            loginSessionContext = new LoginSessionContext
            {
                UserId = loginSessionContext.UserId,
                OsuId = loginSessionContext.OsuId,
                Roles = roles ?? ["User"],
                AccessToken = newTokenQuery.AccessToken,
                RefreshToken = newTokenQuery.RefreshToken,
                ExpiresAt = newTokenQuery.ExpiresAt
            };

            await _loginService.UpdateLoginSessionAsync(sessionId, loginSessionContext);

            Response.Cookies.Append(
                AuthenticationSettings.SessionCookieName,
                sessionId,
                DateTimeOffset.UtcNow.AddDays(_cookieSessionExpiryInDay));
        }

        var claims = loginSessionContext.Roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims.AddRange(
        [
            new Claim(ClaimTypes.NameIdentifier, loginSessionContext.UserId.ToString()),
            new Claim(CustomClaimTypes.OsuId, loginSessionContext.OsuId.ToString()),
            new Claim(CustomClaimTypes.AccessToken, loginSessionContext.AccessToken)
        ]);

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
