namespace EmployeeMgmt.Api.Security;

public sealed class AuthenticationOptions
{
    public string Issuer { get; set; } = "EmployeeMgmt.Api";
    public string Audience { get; set; } = "EmployeeMgmt.Client";
    public string SigningKey { get; set; } = string.Empty;
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string UserUsername { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}
