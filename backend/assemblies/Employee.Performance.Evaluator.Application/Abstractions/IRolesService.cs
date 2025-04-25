using Employee.Performance.Evaluator.Application.RequestsAndResponses.Roles;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IRolesService
{
    Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken);

    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Role> CreateRoleAsync(AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken);

    Task<Role> UpdateRoleAsync(int id, AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken);

    Task<Role> UpdateRolePermissionsAsync(int id, List<int> permissionIds, CancellationToken cancellationToken);

    Task DeleteRoleAsync(int id, CancellationToken cancellationToken);
}
