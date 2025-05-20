using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IEvaluationSessionsRepository : IRepository<EvaluationSession>
{
    Task<List<EvaluationSession>> GetAllWithDetailsAsync(int? employeeId, bool? isFinished, CancellationToken cancellationToken);

    Task<List<EvaluationSession>> GetByEmployeesIdsWithDetailsAsync(List<int>? employeesIds, bool? isFinished, CancellationToken cancellationToken);

    Task<EvaluationSession?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);
}