using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class RoleKPIsRepository : IRoleKPIsRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<RoleKPI> _dbSet;

    public RoleKPIsRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<RoleKPI>();
    }

    public async Task<List<RoleKPI>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await GetWithDetailsInternal().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<RoleKPI?> GetByIdAsync(int roleId, int kpiId, CancellationToken cancellationToken)
    {
        return await GetWithDetailsInternal()
            .FirstOrDefaultAsync(e => e.RoleId == roleId && e.KpiId == kpiId, cancellationToken);
    }

    public async Task<RoleKPI> AddAsync(RoleKPI entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Update(RoleKPI entity) => _dbSet.Update(entity);

    public void Delete(RoleKPI entity) => _dbSet.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

    private IQueryable<RoleKPI> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.KpiMetric)
            .Include(e => e.Role);
    }
}
