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
        var userRole = await _context.Users
            .Where(x => x.Id == userId)
            .Include(x => x.Role)
                .ThenInclude(x => x!.Permissions)
            .Select(x => x.Role)
            .ToArrayAsync(cancellationToken);

        return userRole
            .SelectMany(x => x!.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
