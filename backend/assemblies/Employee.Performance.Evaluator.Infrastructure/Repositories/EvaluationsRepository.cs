using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class EvaluationsRepository : BaseRepository<Evaluation>, IEvaluationsRepository
{
    public EvaluationsRepository(AppDbContext context) : base(context)
    {
    }

    public Task<List<Evaluation>> GetAllBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
    {
        return GetAllWithDetailsInternal()
            .AsNoTracking()
            .Where(e => e.EvaluationSessionId == sessionId)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Evaluation> GetAllWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Evaluator)
                .ThenInclude(ev => ev!.User)
                    .ThenInclude(u => u!.Role)
            .Include(e => e.RoleKpi)
                .ThenInclude(rk => rk!.Role)
            .Include(e => e.RoleKpi)
                .ThenInclude(rk => rk!.KpiMetric);
    }
}
