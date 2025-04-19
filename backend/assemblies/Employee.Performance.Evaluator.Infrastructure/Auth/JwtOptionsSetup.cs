using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        configuration.GetRequiredSection("Jwt").Bind(options);
    }
}
