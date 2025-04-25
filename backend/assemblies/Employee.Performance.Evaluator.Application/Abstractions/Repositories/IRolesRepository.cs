using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IRolesRepository : IRepository<Role>
{
    Task<List<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken);

    Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken cancellationToken);
}
