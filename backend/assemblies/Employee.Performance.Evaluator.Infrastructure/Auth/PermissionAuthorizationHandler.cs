using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        string? userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userId, out int id))
        {
            logger.LogWarning("User ID claim is missing or invalid. User ID: {UserId}", userId);
            context.Fail();
        }

        var userPermissions = context.User.Claims
            .Where(c => c.Type == CustomClaims.PermissionsClaim)
            .Select(c => c.Value)
            .ToHashSet();

        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            logger.LogWarning("User with ID {UserId} does not have permission {Permission} to access this resource.",
                id, requirement.Permission);

            context.Fail();
        }

        return Task.CompletedTask;
    }
}
