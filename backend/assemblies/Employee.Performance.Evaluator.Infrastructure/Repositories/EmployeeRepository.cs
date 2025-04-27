using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context)
    : BaseRepository<EmployeeEntity>(context), IEmployeesRepository
{
    public async Task<EmployeeEntity?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        return await GetWithDetailsInternal().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EmployeeEntity>> GetAllWithDetailsAsync(CancellationToken cancellationToken)
    {
        return await GetWithDetailsInternal().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<EmployeeEntity?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken)
    {
        return await GetWithDetailsInternal().FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }

    public Task<EmployeeEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }

    private IQueryable<EmployeeEntity> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Team)
            .Include(e => e.User)
                .ThenInclude(e => e!.Role)
                .ThenInclude(e => e!.Permissions);
    }
}