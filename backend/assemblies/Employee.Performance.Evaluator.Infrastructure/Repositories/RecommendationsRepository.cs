using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class RecommendationsRepository : BaseRepository<Recommendation>, IRecomendationsRepository
{
    public RecommendationsRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Recommendation>> GetAllWithEmployeeAsync(CancellationToken cancellationToken)
    {
        return await GetRecommendationsInternal()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Recommendation>> GetAllByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken)
    {
        return await GetRecommendationsInternal()
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Recommendation?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        return await GetRecommendationsInternal()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    private IQueryable<Recommendation> GetRecommendationsInternal()
    {
        return _dbSet
            .Include(r => r.Employee)
                .ThenInclude(e => e!.User)
                    .ThenInclude(e => e!.Role);
    }
}
