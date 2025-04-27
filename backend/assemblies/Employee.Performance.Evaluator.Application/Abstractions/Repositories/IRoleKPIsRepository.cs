using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IRoleKPIsRepository
{
    Task<List<RoleKPI>> GetAllAsync(CancellationToken cancellationToken);

    Task<RoleKPI?> GetByIdAsync(int roleId, int kpiId, CancellationToken cancellationToken);

    Task<RoleKPI> AddAsync(RoleKPI entity, CancellationToken cancellationToken);

    void Update(RoleKPI entity);

    void Delete(RoleKPI entity);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
