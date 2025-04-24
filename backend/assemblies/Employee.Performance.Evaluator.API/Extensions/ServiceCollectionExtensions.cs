using System.Text;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Auth;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.Implementations;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Employee.Performance.Evaluator.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Employee.Performance.Evaluator.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<UserStorage>();
        services.AddTransient<IUserIdSetter>(s => s.GetService<UserStorage>()!);
        services.AddTransient<IUserGetter>(s => s.GetService<UserStorage>()!);

        services.AddTransient<IEmployeeService, EmployeesService>();
        services.AddTransient<ITeamsService, TeamsService>();
    }

    public static void AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
    }

    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthServices(configuration);

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ITeamsRepository, TeamsRepository>();
    }

    private static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()!;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                };
            });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
    }
}
