namespace EmployeeMgmt.Api.Security;

public interface ITokenIssuer
{
    string? CreateToken(string username, string password, out DateTime expiresAtUtc);
}
