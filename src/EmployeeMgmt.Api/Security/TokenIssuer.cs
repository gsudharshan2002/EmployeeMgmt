using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeMgmt.Api.Security;

public sealed class TokenIssuer(IOptions<AuthenticationOptions> authenticationOptions) : ITokenIssuer
{
    private readonly AuthenticationOptions _options = authenticationOptions.Value;

    public string? CreateToken(string username, string password, out DateTime expiresAtUtc)
    {
        expiresAtUtc = DateTime.UtcNow.AddMinutes(30);
        var role = Matches(username, _options.AdminUsername) && Matches(password, _options.AdminPassword)
            ? "Admin"
            : Matches(username, _options.UserUsername) && Matches(password, _options.UserPassword)
                ? "Employee"
                : null;

        if (role is null)
            return null;

        var claims = new[]
        {
            new Claim("sub", username),
            new Claim("name", username),
            new Claim("role", role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool Matches(string supplied, string configured)
    {
        if (string.IsNullOrEmpty(configured))
            return false;
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
        var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configured));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, configuredHash);
    }
}
