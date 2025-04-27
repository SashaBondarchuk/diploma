using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class TeamsService(ITeamsRepository teamsRepository) : ITeamsService
{
    public async Task<List<TeamViewModel>> GetAllTeamsAsync(CancellationToken cancellationToken)
    {
        var teams = await teamsRepository.GetAllWithEmployeesAsync(cancellationToken);

        return [.. teams.Select(t => TeamViewModel.MapFromDbModel(t))];
    }

    public async Task<TeamViewModelWithEmployees?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var team = await teamsRepository.GetByIdWithEmployeesAsync(id, cancellationToken);

        return team == null ? null : TeamViewModelWithEmployees.MapFromDbModel(team);
    }

    public async Task<TeamViewModel> CreateTeamAsync(AddUpdateTeamRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("The team name is required.");
        }

        var teams = await teamsRepository.GetAllAsync(cancellationToken);
        if (teams.Any(t => t.Name == request.Name))
        {
            throw new InvalidOperationException($"The team name is already in use.");
        }

        var teamToCreate = new Team() { Name = request.Name };
        var addedTeam = await teamsRepository.AddAsync(teamToCreate, cancellationToken);
        await teamsRepository.SaveChangesAsync(cancellationToken);

        return TeamViewModel.MapFromDbModel(addedTeam);
    }

    public async Task<TeamViewModel> UpdateTeamAsync(int id, AddUpdateTeamRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("The team name is required.");
        }

        var teamToUpdate = await teamsRepository.GetByIdAsync(id, cancellationToken);
        if (teamToUpdate == null)
        {
            throw new InvalidOperationException($"No team with Id={id} found.");
        }

        if (teamToUpdate.Name == request.Name)
        {
            throw new InvalidOperationException($"The team name is already in use.");
        }

        teamToUpdate.Name = request.Name;
        teamsRepository.Update(teamToUpdate);
        await teamsRepository.SaveChangesAsync(cancellationToken);

        return TeamViewModel.MapFromDbModel(teamToUpdate);
    }

    public async Task DeleteTeamAsync(int id, CancellationToken cancellationToken)
    {
        var teamToDelete = await teamsRepository.GetByIdWithEmployeesAsync(id, cancellationToken);
        if (teamToDelete == null)
        {
            throw new InvalidOperationException($"No team with Id={id} found.");
        }

        if ((teamToDelete.Employees?.Count ?? 0) != 0)
        {
            throw new InvalidOperationException($"Cannot delete team with Id={id} because it has employees assigned.");
        }

        teamsRepository.Delete(teamToDelete);
        await teamsRepository.SaveChangesAsync(cancellationToken);
    }
}
