using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IEvaluationsRepository : IRepository<Evaluation>
{
    Task<List<Evaluation>> GetAllBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
}
