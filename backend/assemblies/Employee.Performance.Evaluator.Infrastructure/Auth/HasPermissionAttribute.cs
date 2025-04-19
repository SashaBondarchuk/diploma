using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(UserPermission permission) : base(policy: permission.GetDisplayName())
    {
    }
}
