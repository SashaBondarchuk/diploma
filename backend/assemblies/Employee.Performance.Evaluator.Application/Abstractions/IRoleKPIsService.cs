using Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IRoleKPIsService
{
    Task<List<RoleKPIViewModel>> GetRoleKPIsAsync(CancellationToken cancellationToken);

    Task<RoleKPIViewModel?> GetRoleKPIByIdAsync(int roleId, int kpiId, CancellationToken cancellationToken);

    Task<List<RoleKPIViewModel>> GetAvailableKPIsForSession(int sessionId, User evaluatorUser, CancellationToken cancellationToken);

    Task<RoleKPIViewModel> CreateRoleKPIAsync(AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken);

    Task<RoleKPIViewModel> UpdateRoleKPIAsync(int roleId, int kpiId, AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken);

    Task DeleteRoleKPIAsync(int roleId, int kpiId, CancellationToken cancellationToken);
}
