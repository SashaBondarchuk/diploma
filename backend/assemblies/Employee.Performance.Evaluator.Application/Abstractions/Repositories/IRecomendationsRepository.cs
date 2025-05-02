using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IRecomendationsRepository : IRepository<Recommendation>
{
    Task<List<Recommendation>> GetAllByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken);
    
    Task<List<Recommendation>> GetAllWithEmployeeAsync(CancellationToken cancellationToken);
    
    Task<Recommendation?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);
}
