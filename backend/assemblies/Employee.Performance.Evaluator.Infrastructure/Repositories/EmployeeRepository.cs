using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context)
    : BaseRepository<EmployeeEntity>(context), IEmployeeRepository
{
    public async Task<EmployeeEntity?> GetByIdWithDetailsAsync(int id)
    {
        return await GetWithDetailsInternal().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<EmployeeEntity>> GetAllWithDetailsAsync()
    {
        return await GetWithDetailsInternal().ToListAsync();
    }

    public async Task<EmployeeEntity?> GetByUserIdWithDetailsAsync(int userId)
    {
        return await GetWithDetailsInternal().FirstOrDefaultAsync(e => e.UserId == userId);
    }

    private IQueryable<EmployeeEntity> GetWithDetailsInternal()
    {
        return _dbSet
            .Include(e => e.Team)
            .Include(e => e.User)
            .Include(e => e.Recommendations)
            .Include(e => e.EvaluationSessions);
    }
}