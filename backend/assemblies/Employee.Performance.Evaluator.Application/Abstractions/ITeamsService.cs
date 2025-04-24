using Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface ITeamsService
{
    Task<List<TeamViewModel>> GetAllTeamsAsync(CancellationToken cancellationToken);

    Task<TeamViewModelWithEmployees?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TeamViewModel> CreateTeamAsync(AddUpdateTeamRequest request, CancellationToken cancellationToken);

    Task<TeamViewModel> UpdateTeamAsync(int id, AddUpdateTeamRequest request, CancellationToken cancellationToken);

    Task DeleteTeamAsync(int id, CancellationToken cancellationToken);
}
