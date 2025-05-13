using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class TeamsRepository : BaseRepository<Team>, ITeamsRepository
{
    public TeamsRepository(AppDbContext context) : base(context)
    {
    }

    public Task<List<Team>> GetAllWithTeamLeadAsync(CancellationToken cancellationToken)
    {
        return _dbSet
            .Include(e => e.TeamLead)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Team?> GetByIdWithEmployeesAsync(int id, CancellationToken cancellationToken)
    {
        return GetWithDetailsInternal()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    private IQueryable<Team> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Employees!)
                .ThenInclude(e => e.User)
                    .ThenInclude(e => e!.Role);
    }
}
