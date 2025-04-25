using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class RolesRepository : BaseRepository<Role>, IRolesRepository
{
    public RolesRepository(AppDbContext context) : base(context)
    {
    }

    public Task<List<Role>> GetAllWithPermissionsAsync(CancellationToken cancellationToken)
    {
        return GetWithDetailsInternal()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Role?> GetByIdWithPermissionsAsync(int id, CancellationToken cancellationToken)
    {
        return GetWithDetailsInternal()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    private IQueryable<Role> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Permissions);
    }
}
