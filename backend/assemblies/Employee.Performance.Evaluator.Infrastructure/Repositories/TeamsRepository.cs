using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Infrastructure.Context;

namespace Employee.Performance.Evaluator.Infrastructure.Repositories;

public class TeamsRepository : BaseRepository<Team>, ITeamsRepository
{
    public TeamsRepository(AppDbContext context) : base(context)
    {
    }
}
