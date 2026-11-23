using System.ComponentModel.DataAnnotations;
using EmployeeMgmt.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMgmt.Api.Controllers;

public sealed class LoginRequest
{
    [Required, StringLength(100)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(256)]
    public string Password { get; init; } = string.Empty;
}

public sealed record TokenResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ITokenIssuer tokenIssuer) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenResponse> CreateToken(LoginRequest request)
    {
        var token = tokenIssuer.CreateToken(request.Username, request.Password, out var expiresAtUtc);
        return token is null
            ? Unauthorized()
            : Ok(new TokenResponse(token, "Bearer", expiresAtUtc));
    }
}
