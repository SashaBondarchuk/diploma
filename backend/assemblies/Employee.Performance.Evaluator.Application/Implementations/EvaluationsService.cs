using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Evaluations;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EvaluationsService(
    IUserGetter userGetter,
    IEmployeesRepository employeesRepository,
    IRoleKPIsService roleKPIsService,
    IEvaluationSessionsRepository evaluationSessionsRepository,
    IEvaluationsRepository evaluationsRepository) : IEvaluationsService
{
    public async Task<List<EvaluationViewModel>> GetAllBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
    {
        var evaluations = await evaluationsRepository.GetAllBySessionIdAsync(sessionId, cancellationToken);

        return [.. evaluations.Select(EvaluationViewModel.MapFromDbModel)];
    }

    public async Task CreateEvaluationAsync(AddEvaluationRequest addEvaluationRequest, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(
            addEvaluationRequest.EvaluationSessionId, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={addEvaluationRequest.EvaluationSessionId} not found.");
        }
        if (evaluationSession.EndDate < DateTimeOffset.Now)
        {
            throw new InvalidOperationException($"Evaluation session with Id={evaluationSession.Id} has already ended.");
        }
        if (evaluationSession.Employee!.User!.RoleId != addEvaluationRequest.RoleId)
        {
            throw new InvalidOperationException($"Employee with Id={evaluationSession.Employee.Id} does not have RoleId={addEvaluationRequest.RoleId}.");
        }

        var evaluatorUser = userGetter.GetCurrentUserOrThrow();
        var evaluatorEmployee = await employeesRepository.GetByUserIdAsync(evaluatorUser.Id, cancellationToken);

        if (evaluationSession.Employee!.TeamId != evaluatorEmployee!.TeamId) //manager access
        {
            throw new InvalidOperationException($"Evaluator with Id={evaluatorEmployee.Id} is not in the same team as the evaluated employee with Id={evaluationSession.Employee.Id}.");
        }

        var availableKpis = await roleKPIsService.GetAvailableKPIsForSession(
                addEvaluationRequest.EvaluationSessionId, evaluatorUser, cancellationToken);

        var kpiMetricToEvaluate = availableKpis.FirstOrDefault(rkpi => rkpi.KpiId == addEvaluationRequest.KpiId);
        if (kpiMetricToEvaluate == null)
        {
            throw new InvalidOperationException($"Evaluator with Id={evaluatorEmployee!.Id} is not allowed to evaluate KpiId={addEvaluationRequest.KpiId}.");
        }
        if (kpiMetricToEvaluate.MinScore > addEvaluationRequest.Score || kpiMetricToEvaluate.MaxScore < addEvaluationRequest.Score)
        {
            throw new InvalidOperationException($"Score {addEvaluationRequest.Score} is not in the range of {kpiMetricToEvaluate.MinScore} - {kpiMetricToEvaluate.MaxScore}.");
        }

        var evaluationToCreate = new Evaluation()
        {
            RoleId = addEvaluationRequest.RoleId,
            KpiId = addEvaluationRequest.KpiId,
            EvaluationSessionId = addEvaluationRequest.EvaluationSessionId,
            Score = addEvaluationRequest.Score,
            Comment = addEvaluationRequest.Comment ?? string.Empty,
            EvaluatorId = evaluatorEmployee!.Id,
        };

        var addedEvaluation = await evaluationsRepository.AddAsync(evaluationToCreate, cancellationToken);
        await evaluationsRepository.SaveChangesAsync(cancellationToken);
    }
}
