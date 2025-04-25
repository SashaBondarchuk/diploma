using System.Security.Claims;
using Employee.Performance.Evaluator.Application.Abstractions;

namespace Employee.Performance.Evaluator.API.Middlewares;

public class UserSaverMiddleware(RequestDelegate next, ILogger<UserSaverMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUserIdSetter userSetter)
    {
        string? userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userId, out int id))
        {
            await userSetter.SetUserIdAsync(id, CancellationToken.None);
        }
        else
        {
            logger.LogWarning("User ID claim is missing or invalid. User ID: {UserId}", userId);
        }

        await next.Invoke(context);
    }
}
