using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class PermissionsRepository : BaseRepository<Permission>, IPermissionsRepository
{
    public PermissionsRepository(AppDbContext context) : base(context)
    {
    }
}
