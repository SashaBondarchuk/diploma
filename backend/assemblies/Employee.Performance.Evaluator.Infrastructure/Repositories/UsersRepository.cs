using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class UsersRepository : BaseRepository<User>, IUsersRepository
{
    public UsersRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<HashSet<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken)
    {
        var userRole = await GetWithDetailsInternal()
            .Where(x => x.Id == userId)
            .Select(x => x.Role)
            .FirstAsync(cancellationToken);

        return [.. userRole!.Permissions.Select(x => x.Name)];
    }

    public Task<List<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
    {
        return GetWithDetailsInternal().ToListAsync(cancellationToken);
    }

    public Task<User?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        return GetWithDetailsInternal()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    private IQueryable<User> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Role)
                .ThenInclude(e => e!.Permissions);
    }
}
