using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Internal;
using SS14.Auth.Shared;
using SS14.Auth.Shared.Data;
using SS14.Auth.Shared.Emails;
using SS14.Auth.Shared.Sessions;

namespace SS14.Auth.Controllers;

/// <summary>
///     Contains the API endpoints used by the launcher to log in and such.
/// </summary>
[ApiController]
[Route("/api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly SessionManager _sessionManager;
    private readonly IEmailSender _emailSender;
    private readonly ISystemClock _systemClock;
    private readonly IConfiguration _cfg;

    private readonly SpaceUserManager _userManager;
    private readonly SignInManager<SpaceUser> _signInManager;

    private string WebBaseUrl => _cfg.GetValue<string>("WebBaseUrl") ?? "";

    public AuthApiController(SpaceUserManager userManager, SignInManager<SpaceUser> signInManager,
        SessionManager sessionManager, IEmailSender emailSender, ISystemClock systemClock, IConfiguration cfg)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _sessionManager = sessionManager;
        _emailSender = emailSender;
        _systemClock = systemClock;
        _cfg = cfg;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        // Password may never be null, and only either username OR userID can be used for login, not both.
        if (!(request.Username == null ^ request.UserId == null))
        {
            return BadRequest();
        }

        // Console.WriteLine(Request.Headers["SS14-Launcher-Fingerprint"]);
        // Console.WriteLine(Request.Headers["User-Agent"]);

        SpaceUser? user;
        if (request.Username != null)
        {
            user = await _userManager.FindByNameOrEmailAsync(request.Username);
        }
        else
        {
            Debug.Assert(request.UserId != null);

            user = await _userManager.FindByIdAsync(request.UserId!.Value.ToString());
        }

        if (user == null)
        {
            return Unauthorized(new AuthenticateDenyResponse(
                new[] { "Invalid login credentials." },
                AuthenticateDenyResponseCode.InvalidCredentials));
        }

        var emailUnconfirmed = _userManager.Options.SignIn.RequireConfirmedEmail &&
                               !await _userManager.IsEmailConfirmedAsync(user);

        if (emailUnconfirmed)
        {
            return Unauthorized(new AuthenticateDenyResponse(new[]
            {
                "The email address for this account still needs to be confirmed. " +
                "Please confirm your email address before trying to log in."
            }, AuthenticateDenyResponseCode.AccountUnconfirmed));
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult is SpaceSignInResult { IsAdminLocked: true })
        {
            return Unauthorized(new AuthenticateDenyResponse(
                new[] { "Account locked by administrator." },
                AuthenticateDenyResponseCode.AccountLocked));
        }

        if (signInResult is SpaceSignInResult { EmailChangeRequired: true })
        {
            return Unauthorized(new AuthenticateDenyResponse(
                new[] { "Account requires a new email." },
                AuthenticateDenyResponseCode.EmailChangeNeeded));
        }

        if (signInResult is SpaceSignInResult { PasswordChangeRequired: true })
        {
            return Unauthorized(new AuthenticateDenyResponse(
                new[] { "Account requires a new password." },
                AuthenticateDenyResponseCode.PasswdChangeNeeded));
        }

        if (!signInResult.Succeeded)
        {
            return Unauthorized(new AuthenticateDenyResponse(
                new[] { "Invalid login credentials." },
                AuthenticateDenyResponseCode.InvalidCredentials));
        }

        if (user.TwoFactorEnabled)
        {
            if (request.TfaCode == null)
            {
                // No 2FA code given, presumably the first login attempt.
                return Unauthorized(new AuthenticateDenyResponse(
                    new[] { "" },
                    AuthenticateDenyResponseCode.TfaRequired));
            }

            var verify = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.TfaCode);

            if (!verify)
            {
                return Unauthorized(new AuthenticateDenyResponse(
                    new[] { "" },
                    AuthenticateDenyResponseCode.TfaInvalid));
            }

            // 2FA passed, we're good.
        }

        var (token, expireTime) =
            await _sessionManager.RegisterNewSession(user, SessionManager.DefaultExpireTime);

        return Ok(new AuthenticateResponse(token.AsBase64, user.UserName!, user.Id, expireTime));
    }

    // Launcher registration disabled due to spam risk.
    /*
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var userName = request.Username.Trim();
        var email = request.Email.Trim();

        var user = ModelShared.CreateNewUser(userName, email, _systemClock);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(p => p.Description).ToArray();
            return UnprocessableEntity(new RegisterResponseError(errors));
        }

        var confirmLink = await GenerateEmailConfirmLink(user);

        await ModelShared.SendConfirmEmail(_emailSender, email, confirmLink);

        var status = _userManager.Options.SignIn.RequireConfirmedAccount
            ? RegisterResponseStatus.RegisteredNeedConfirmation
            : RegisterResponseStatus.Registered;

        return Ok(new RegisterResponse(status));
    }
    */

    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Ok();
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = $"{WebBaseUrl}Identity/Account/ResetPassword?code={code}&launcher=true";

        await ModelShared.SendResetEmail(_emailSender, email, callbackUrl);

        return Ok();
    }

    // Launcher resend confirmation disabled due to spam risk.
    /*
    [HttpPost("resendConfirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Ok();
        }

        var confirmLink = await GenerateEmailConfirmLink(user);

        await ModelShared.SendConfirmEmail(_emailSender, email, confirmLink);

        return Ok();
    }
    */

    [Authorize(AuthenticationSchemes = "SS14Auth")]
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var user = await _userManager.GetUserAsync(User);

        return Ok($"Hi, {user!.UserName}");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        if (!SessionToken.TryFromBase64(request.Token, out var token))
        {
            return BadRequest();
        }

        await _sessionManager.InvalidateToken(token);
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        if (!SessionToken.TryFromBase64(request.Token, out var token))
        {
            return BadRequest();
        }

        var refreshed = await _sessionManager.RefreshToken(token);
        if (refreshed == null)
        {
            return Unauthorized();
        }

        var (newToken, expireTime) = refreshed.Value;

        return Ok(new RefreshResponse(expireTime, newToken.AsBase64));
    }

    private async Task<string> GenerateEmailConfirmLink(SpaceUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var baseUrl = WebBaseUrl;
        return $"{baseUrl}Identity/Account/ConfirmEmail?code={code}&userId={user.Id}&launcher=true";
    }
}

public sealed record AuthenticateRequest(
    string? Username,
    Guid? UserId,
    string Password,
    string? TfaCode = null);

public sealed record AuthenticateResponse(string Token, string Username, Guid UserId, DateTimeOffset ExpireTime)
{
}

public sealed record AuthenticateDenyResponse(string[] Errors, AuthenticateDenyResponseCode Code);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticateDenyResponseCode
{
    // @formatter:off
    None               = 0,
    InvalidCredentials = 1,
    AccountUnconfirmed = 2,
    TfaRequired        = 3,
    TfaInvalid         = 4,
    AccountLocked      = 5,
    EmailChangeNeeded  = 6,
    PasswdChangeNeeded = 7,
    // @formatter:on
}

public sealed record RegisterRequest(string Username, string Email, string Password)
{
}

public sealed record ResetPasswordRequest(string Email)
{
}

public sealed record ResendConfirmationRequest(string Email)
{
}

public sealed record RegisterResponse(RegisterResponseStatus Status)
{
}

public sealed record RegisterResponseError(string[] Errors)
{
}

public sealed record LogoutRequest(string Token)
{
}

public sealed record RefreshRequest(string Token)
{
}

public sealed record RefreshResponse(DateTimeOffset ExpireTime, string NewToken)
{
}

public enum RegisterResponseStatus
{
    Registered,
    RegisteredNeedConfirmation
}
