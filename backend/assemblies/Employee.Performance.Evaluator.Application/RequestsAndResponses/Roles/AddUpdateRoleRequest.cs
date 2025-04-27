namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Roles;

public class AddUpdateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;

    public List<int> PermissionIds { get; set; } = [];
}
