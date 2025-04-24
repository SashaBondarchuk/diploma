using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface ITeamsRepository : IRepository<Team>
{
    public Task<Team?> GetByIdWithEmployees(int id, CancellationToken cancellationToken);
}
