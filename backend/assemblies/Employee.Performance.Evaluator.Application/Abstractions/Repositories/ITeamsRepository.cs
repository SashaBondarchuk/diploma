using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface ITeamsRepository : IRepository<Team>
{
    Task<List<Team>> GetAllWithEmployeesAsync(CancellationToken cancellationToken);

    Task<Team?> GetByIdWithEmployeesAsync(int id, CancellationToken cancellationToken);
}
