namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public int ExpirationTimeInHours { get; set; } = 1;
}
