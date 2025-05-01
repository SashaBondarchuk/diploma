using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EvaluationSessionsService(
    IEmployeesRepository employeesRepository,
    IEvaluationSessionsRepository evaluationSessionsRepository) : IEvaluationSessionsService
{
    public async Task<List<EvaluationSessionViewModel>> GetAllWithDetailsAsync(
        int? employeeId,
        bool? isFinished,
        CancellationToken cancellationToken)
    {
        var evaluationSessions = await evaluationSessionsRepository.GetAllWithDetailsAsync(employeeId, isFinished, cancellationToken);

        return [.. evaluationSessions.Select(e => EvaluationSessionViewModel.MapFromDbModel(e))];
    }

    public async Task<List<EvaluationSessionViewModel>> GetOngoingEvaluationsForEmployeeTeamMembersAsync(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var employee = await employeesRepository.GetByIdWithDetailsAndTeamMembersAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new InvalidOperationException($"Employee with Id={employeeId} not found.");
        }

        var teamMembersIds = employee.Team!.Employees!.Where(e => e.Id != employee.Id).Select(e => e.Id).ToList();
        var evaluationSessions = await evaluationSessionsRepository.GetByEmployeesIdsWithDetailsAsync(
            teamMembersIds, isFinished: false, cancellationToken);

        return [.. evaluationSessions.Select(e => EvaluationSessionViewModel.MapFromDbModel(e))];
    }

    public async Task<EvaluationSessionViewModel?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return evaluationSession != null ? EvaluationSessionViewModel.MapFromDbModel(evaluationSession) : null;
    }

    public async Task<EvaluationSessionViewModel> CreateEvaluationSessionAsync(AddEvaluationSessionRequest addEvaluationSessionRequest, CancellationToken cancellationToken)
    {
        if (addEvaluationSessionRequest.EndDate < DateTimeOffset.Now.AddDays(1))
        {
            throw new InvalidOperationException("End date must be at least one day in the future.");
        }

        var employeeExists = await employeesRepository.ExistsAsync(addEvaluationSessionRequest.EmployeeId, cancellationToken);
        if (!employeeExists)
        {
            throw new InvalidOperationException($"Employee with Id={addEvaluationSessionRequest.EmployeeId} not found.");
        }

        var evaluationSessionToCreate = new EvaluationSession()
        {
            Name = addEvaluationSessionRequest.Name,
            EmployeeId = addEvaluationSessionRequest.EmployeeId,
            StartDate = DateTimeOffset.Now,
            EndDate = addEvaluationSessionRequest.EndDate,
        };

        var addedEvaluationSession = await evaluationSessionsRepository.AddAsync(evaluationSessionToCreate, cancellationToken);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var addedEvaluationSessionWithDetails = await evaluationSessionsRepository.GetByIdWithDetailsAsync(addedEvaluationSession.Id, cancellationToken);

        return EvaluationSessionViewModel.MapFromDbModel(addedEvaluationSessionWithDetails!);
    }

    //inprogress
    public async Task<EvaluationSessionViewModel> EndEvaluationSessionAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdAsync(id, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={id} not found.");
        }

        if (evaluationSession.EndDate > DateTimeOffset.Now)
        {
            throw new InvalidOperationException($"Evaluation session cannot be ended before the end date.");
        }

        if (evaluationSession.EvaluationFinishedDate != null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={id} is already finished.");
        }

        evaluationSession.EvaluationFinishedDate = DateTimeOffset.Now;

        // Calculate the average score for the evaluation session
        // evaluationSession.ClassId = 
        // evaluationSession.WeightedScore = 

        evaluationSessionsRepository.Update(evaluationSession);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var updatedEvaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return EvaluationSessionViewModel.MapFromDbModel(updatedEvaluationSession!);
    }

    //ingprogress
    public async Task<EvaluationSessionViewModel> GenerateReportForSessionAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdAsync(id, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={id} not found.");
        }

        if (evaluationSession.EvaluationFinishedDate == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={id} is not finished yet.");
        }

        // Generate the report and save it to the database
        // evaluationSession.ReportFile = 

        evaluationSessionsRepository.Update(evaluationSession);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var updatedEvaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return EvaluationSessionViewModel.MapFromDbModel(updatedEvaluationSession!);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdAsync(id, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={id} not found.");
        }

        evaluationSessionsRepository.Delete(evaluationSession);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);
    }
}
