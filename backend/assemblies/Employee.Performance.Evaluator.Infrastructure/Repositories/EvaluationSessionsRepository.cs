using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class EvaluationSessionsRepository : BaseRepository<EvaluationSession>, IEvaluationSessionsRepository
{
    public EvaluationSessionsRepository(AppDbContext context) : base(context)
    {
    }

    public Task<List<EvaluationSession>> GetAllWithDetailsAsync(int? employeeId, bool? isFinished, CancellationToken cancellationToken)
    {
        return GetAllWithDetails()
            .AsNoTracking()
            .Where(e => !employeeId.HasValue || e.EmployeeId == employeeId)
            .Where(e => !isFinished.HasValue || ((bool)isFinished ? e.EvaluationFinishedDate != null : e.EvaluationFinishedDate == null))
            .ToListAsync(cancellationToken);
    }

    public Task<List<EvaluationSession>> GetByEmployeesIdsWithDetailsAsync(List<int>? employeesIds, bool? isFinished, CancellationToken cancellationToken)
    {
        return GetAllWithDetails()
            .AsNoTracking()
            .Where(e =>
                (employeesIds == null || employeesIds.Count == 0 || employeesIds.Contains(e.EmployeeId)) &&
                (!isFinished.HasValue || (isFinished.Value ? e.EvaluationFinishedDate != null : e.EvaluationFinishedDate == null))
            )
            .ToListAsync(cancellationToken);
    }

    public Task<EvaluationSession?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        return GetAllWithDetails().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    private IQueryable<EvaluationSession> GetAllWithDetails()
    {
        return _dbSet
            .Include(e => e.Employee)
                .ThenInclude(e => e!.Team!)
            .Include(e => e.Employee)
                .ThenInclude(e => e!.User)
                    .ThenInclude(u => u!.Role)
            .Include(e => e.Class);
    }
}
