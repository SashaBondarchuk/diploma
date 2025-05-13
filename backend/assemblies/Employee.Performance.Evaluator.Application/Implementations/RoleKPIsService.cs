using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class RoleKPIsService(
    IRoleKPIsRepository roleKPIsRepository,
    IRolesRepository rolesRepository,
    IEmployeesRepository employeesRepository,
    IUsersService usersService,
    IEvaluationSessionsRepository evaluationSessionsRepository,
    IEvaluationsRepository evaluationsRepository,
    IKPIMetricsRepository kPIMetricsRepository) : IRoleKPIsService
{
    public async Task<List<RoleKPIViewModel>> GetRoleKPIsAsync(CancellationToken cancellationToken)
    {
        var roleKPIs = await roleKPIsRepository.GetAllAsync(cancellationToken);

        return [.. roleKPIs.Select(r => RoleKPIViewModel.MapFromDbModel(r))];
    }

    public async Task<RoleKPIViewModel?> GetRoleKPIByIdAsync(int roleId, int kpiId, CancellationToken cancellationToken)
    {
        var roleKPI = await roleKPIsRepository.GetByIdAsync(roleId, kpiId, cancellationToken);

        return roleKPI != null ? RoleKPIViewModel.MapFromDbModel(roleKPI) : null;
    }

    public async Task<List<RoleKPIViewModel>> GetAvailableKPIsForSession(int sessionId, User evaluatorUser, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(sessionId, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={sessionId} not found.");
        }

        if (evaluationSession.EndDate < DateTimeOffset.Now)
        {
            throw new InvalidOperationException($"Evaluation session with Id={sessionId} has already ended.");
        }

        var evaluator = await employeesRepository.GetByUserIdAsync(evaluatorUser.Id, cancellationToken);
        var sessionEvaluations = await evaluationsRepository.GetAllBySessionIdAsync(sessionId, cancellationToken);

        var evaluatedKpiPairs = sessionEvaluations
            .Where(s => s.EvaluatorId == evaluator!.Id)
            .Select(e => (e.RoleKpi!.RoleId, e.RoleKpi.KpiId))
            .ToHashSet();

        var allRoleKpis = await roleKPIsRepository.GetAllByRoleIdAsync(evaluationSession.Employee!.User!.RoleId, cancellationToken);

        var evaluatorHasTeamLeadPermissions = usersService.HasTeamLeadPermissions(evaluatorUser);
        var availableToEvaluateKpis = allRoleKpis
            .Where(rkpi => !evaluatedKpiPairs.Contains((rkpi.RoleId, rkpi.KpiId)))
            .Where(rkpi => evaluatorHasTeamLeadPermissions || rkpi.IsAllowedToEvaluateExceptLead)
            .Select(RoleKPIViewModel.MapFromDbModel)
            .ToList();

        return availableToEvaluateKpis;
    }

    public async Task<RoleKPIViewModel> CreateRoleKPIAsync(AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken)
    {
        var roleExists = await rolesRepository.ExistsAsync(addUpdateRoleKPIRequest.RoleId, cancellationToken);
        if (!roleExists)
        {
            throw new InvalidOperationException($"The Role with Id={addUpdateRoleKPIRequest.RoleId} does not exist.");
        }

        var kpiExists = await kPIMetricsRepository.ExistsAsync(addUpdateRoleKPIRequest.KpiId, cancellationToken);
        if (!kpiExists)
        {
            throw new InvalidOperationException($"The KPI with Id={addUpdateRoleKPIRequest.KpiId} does not exist.");
        }

        var existingRoleKPI = await roleKPIsRepository.GetByIdAsync(addUpdateRoleKPIRequest.RoleId, addUpdateRoleKPIRequest.KpiId, cancellationToken);
        if (existingRoleKPI != null)
        {
            throw new InvalidOperationException("The RoleKPI already exists.");
        }

        var roleKPIToCreate = new RoleKPI
        {
            RoleId = addUpdateRoleKPIRequest.RoleId,
            KpiId = addUpdateRoleKPIRequest.KpiId,
            Weight = addUpdateRoleKPIRequest.Weight,
            IsAllowedToEvaluateExceptLead = addUpdateRoleKPIRequest.IsAllowedToEvaluateExceptLead,
            MinScore = addUpdateRoleKPIRequest.MinScore,
            MaxScore = addUpdateRoleKPIRequest.MaxScore,
            ScoreRangeDescription = addUpdateRoleKPIRequest.ScoreRangeDescription
        };

        var addedRoleKPI = await roleKPIsRepository.AddAsync(roleKPIToCreate, cancellationToken);
        await roleKPIsRepository.SaveChangesAsync(cancellationToken);

        var addedRoleKPIWithDetails = await roleKPIsRepository.GetByIdAsync(addedRoleKPI.RoleId, addedRoleKPI.KpiId, cancellationToken);
        return RoleKPIViewModel.MapFromDbModel(addedRoleKPIWithDetails!);
    }

    public async Task<RoleKPIViewModel> UpdateRoleKPIAsync(int roleId, int kpiId, AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken)
    {
        var roleExists = await rolesRepository.ExistsAsync(addUpdateRoleKPIRequest.RoleId, cancellationToken);
        if (!roleExists)
        {
            throw new InvalidOperationException($"The Role with Id={addUpdateRoleKPIRequest.RoleId} does not exist.");
        }

        var kpiExists = await kPIMetricsRepository.ExistsAsync(addUpdateRoleKPIRequest.KpiId, cancellationToken);
        if (!kpiExists)
        {
            throw new InvalidOperationException($"The KPI with Id={addUpdateRoleKPIRequest.KpiId} does not exist.");
        }

        var roleKPIToUpdate = await roleKPIsRepository.GetByIdAsync(roleId, kpiId, cancellationToken);
        if (roleKPIToUpdate == null)
        {
            throw new InvalidOperationException("The RoleKPI does not exist.");
        }

        roleKPIToUpdate.Weight = addUpdateRoleKPIRequest.Weight;
        roleKPIToUpdate.IsAllowedToEvaluateExceptLead = addUpdateRoleKPIRequest.IsAllowedToEvaluateExceptLead;
        roleKPIToUpdate.MinScore = addUpdateRoleKPIRequest.MinScore;
        roleKPIToUpdate.MaxScore = addUpdateRoleKPIRequest.MaxScore;
        roleKPIToUpdate.ScoreRangeDescription = addUpdateRoleKPIRequest.ScoreRangeDescription;

        roleKPIsRepository.Update(roleKPIToUpdate);
        await roleKPIsRepository.SaveChangesAsync(cancellationToken);

        var updatedRoleKPIWithDetails = await roleKPIsRepository.GetByIdAsync(roleKPIToUpdate.RoleId, roleKPIToUpdate.KpiId, cancellationToken);
        return RoleKPIViewModel.MapFromDbModel(updatedRoleKPIWithDetails!);
    }

    public async Task DeleteRoleKPIAsync(int roleId, int kpiId, CancellationToken cancellationToken)
    {
        var roleKPIToDelete = await roleKPIsRepository.GetByIdAsync(roleId, kpiId, cancellationToken);
        if (roleKPIToDelete == null)
        {
            throw new InvalidOperationException("The RoleKPI does not exist.");
        }

        try
        {
            roleKPIsRepository.Delete(roleKPIToDelete);
            await roleKPIsRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete RoleKPI with Id={roleId} and KpiId={kpiId}. It might be in use by other entities.", ex);
        }
    }
}
