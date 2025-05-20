using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Reporting;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EvaluationSessionsService(
    IReportsService reportsService,
    IRoleKPIsService roleKPIsService,
    IEmployeesRepository employeesRepository,
    IEvaluationsRepository evaluationsRepository,
    IEmployeeClassesRepository employeeClassesRepository,
    IEvaluationSessionsRepository evaluationSessionsRepository) : IEvaluationSessionsService
{
    public async Task<List<EvaluationSessionViewModel>> GetAllWithDetailsAsync(int? employeeId, bool? isFinished, CancellationToken cancellationToken)
    {
        var evaluationSessions = await evaluationSessionsRepository.GetAllWithDetailsAsync(employeeId, isFinished, cancellationToken);

        return [.. evaluationSessions.Select(e => EvaluationSessionViewModel.MapFromDbModel(e))];
    }

    public async Task<List<EvaluationSessionViewModel>> GetOngoingEvaluationsForEmployeeAsync(int employeeId, User currentUser, CancellationToken cancellationToken)
    {
        var employee = await employeesRepository.GetByIdWithDetailsAndTeamMembersAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new InvalidOperationException($"Employee with Id={employeeId} not found.");
        }

        var isManager = !currentUser.Role!.Permissions.Any(p => p.Id == (int)UserPermission.ManageEvaluations);
        var teamMembersIds = isManager ? employee.Team!.Employees!.Where(e => e.Id != employee.Id).Select(e => e.Id).ToList() : null;
        var evaluationSessions = await evaluationSessionsRepository.GetByEmployeesIdsWithDetailsAsync(
            teamMembersIds, isFinished: false, cancellationToken);

        var result = new List<EvaluationSessionViewModel>();

        foreach (var evaluationSession in evaluationSessions)
        {
            var roleKPIs = await roleKPIsService.GetAvailableKPIsForSession(evaluationSession.Id, currentUser, cancellationToken);
            if (roleKPIs != null && roleKPIs.Count > 0)
            {
                result.Add(EvaluationSessionViewModel.MapFromDbModel(evaluationSession));
            }
        }

        return result;
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
            ReportFile = null,
        };

        var addedEvaluationSession = await evaluationSessionsRepository.AddAsync(evaluationSessionToCreate, cancellationToken);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var addedEvaluationSessionWithDetails = await evaluationSessionsRepository.GetByIdWithDetailsAsync(addedEvaluationSession.Id, cancellationToken);

        return EvaluationSessionViewModel.MapFromDbModel(addedEvaluationSessionWithDetails!);
    }

    public async Task<EvaluationSessionViewModel> EndEvaluationSessionAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await GetSessionWithValidation(id, cancellationToken);

        var evaluations = await evaluationsRepository.GetAllBySessionIdAsync(id, cancellationToken);
        if (evaluations == null || evaluations.Count == 0)
        {
            throw new InvalidOperationException($"Cannot end the session with Id={id} with no evaluations");
        }

        var avarageScoreForEachRoleKpi = CalculateAverageScoreForEachRoleKpi(evaluations);
        var weightedScore = avarageScoreForEachRoleKpi.Select(rkpiScore => rkpiScore.AverageScore * rkpiScore.KpiWeight).Sum();

        var employeeClasses = await employeeClassesRepository.GetAllAsync(cancellationToken);
        var employeeClass = employeeClasses.FirstOrDefault(ec => weightedScore >= ec.MinScore && weightedScore <= ec.MaxScore); // needs validation

        evaluationSession.WeightedScore = Math.Round(weightedScore, 4);
        evaluationSession.EvaluationFinishedDate = DateTimeOffset.Now;
        evaluationSession.ClassId = employeeClass!.Id;

        evaluationSessionsRepository.Update(evaluationSession);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var updatedEvaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return EvaluationSessionViewModel.MapFromDbModel(updatedEvaluationSession!);
    }

    public async Task<EvaluationSessionViewModel> GenerateReportForSessionAsync(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await GetSessionWithValidation(id, cancellationToken);
        var evaluations = await evaluationsRepository.GetAllBySessionIdAsync(id, cancellationToken);

        var allSessions = (await GetAllFinishedSessionsForEmployee(evaluationSession.EmployeeId, cancellationToken))
            .Where(es => es.EvaluationFinishedDate <= evaluationSession.EvaluationFinishedDate).ToList();


        var allEvaluations = await evaluationsRepository.GetAllWithDetailsAsync(cancellationToken);

        var kpiHistoryAndForecast = await PrepareKpiHistoryAndForecast(allSessions, allEvaluations, evaluationSession, cancellationToken);

        evaluationSession.ReportFile = reportsService.GeneratePdfReport(
            evaluationSession,
            evaluations,
            kpiHistoryAndForecast.KpiTable,
            kpiHistoryAndForecast.SessionNames,
            kpiHistoryAndForecast.CanForecast,
            kpiHistoryAndForecast.ForecastedWeightedScore
        ).ToArray();

        evaluationSessionsRepository.Update(evaluationSession);
        await evaluationSessionsRepository.SaveChangesAsync(cancellationToken);

        var updatedEvaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return EvaluationSessionViewModel.MapFromDbModel(updatedEvaluationSession!);
    }

    public async Task<(byte[] content, string fileName)> GetReportAsync(int sessionId, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(sessionId, cancellationToken);
        if (evaluationSession == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={sessionId} not found.");
        }

        if (evaluationSession.ReportFile == null)
        {
            throw new InvalidOperationException($"Evaluation session with Id={sessionId} does not have a report file.");
        }

        var employee = evaluationSession.Employee!;
        var finishedDateUnix = evaluationSession.EvaluationFinishedDate?.ToUnixTimeSeconds();
        var reportName = $"Session #{finishedDateUnix} {evaluationSession.Name}; Employee {employee.FirstName} {employee.LastName}.pdf";
        return (evaluationSession.ReportFile, reportName);
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

    private async Task<KpiHistoryAndForecastResult> PrepareKpiHistoryAndForecast(List<EvaluationSession> sessions, List<Evaluation> allEvaluations, EvaluationSession evaluationSession, CancellationToken cancellationToken)
    {
        var kpiHistory = new Dictionary<int, List<(string SessionName, decimal Score)>>();
        var sessionNames = new List<string>();

        foreach (var session in sessions)
        {
            sessionNames.Add(session.Name);

            var sessionEvals = allEvaluations.Where(e => e.EvaluationSessionId == session.Id).ToList();
            var kpiScores = CalculateAverageScoreForEachRoleKpi(sessionEvals);

            foreach (var kpi in kpiScores)
            {
                if (!kpiHistory.ContainsKey(kpi.KpiId))
                    kpiHistory[kpi.KpiId] = new List<(string, decimal)>();

                kpiHistory[kpi.KpiId].Add((session.Name, Math.Round(kpi.AverageScore, 2)));
            }
        }

        decimal? forecastedWeightedScore = null;
        var forecastedKpiMetricsPerformances = new List<KpiMetricPerformance>();
        var canForecast = sessions.Count >= 3;
        if (canForecast)
        {
            forecastedKpiMetricsPerformances = await ForecastNextPeriodPerformance(sessions, cancellationToken);
            forecastedWeightedScore = forecastedKpiMetricsPerformances.Select(rkpiScore => rkpiScore.AverageScore * rkpiScore.KpiWeight).Sum();
        }

        if (canForecast)
        {
            sessionNames.Add("Forecast");
            foreach (var kpi in forecastedKpiMetricsPerformances)
            {
                if (!kpiHistory.ContainsKey(kpi.KpiId))
                    kpiHistory[kpi.KpiId] = new List<(string, decimal)>();
                kpiHistory[kpi.KpiId].Add(("Forecast", Math.Round(kpi.AverageScore, 2)));
            }
        }

        var kpiTable = kpiHistory.Select(kvp => new KpiHistoryRow
        {
            KpiId = kvp.Key,
            KpiName = kvp.Value.First().SessionName == null ? "" : allEvaluations.FirstOrDefault(e => e.KpiId == kvp.Key)?.RoleKpi?.KpiMetric?.Name ?? "",
            Scores = kvp.Value.Select(x => x.Score).ToList()
        }).ToList();

        return new KpiHistoryAndForecastResult
        {
            KpiTable = kpiTable,
            SessionNames = sessionNames,
            CanForecast = canForecast,
            ForecastedKpiMetricsPerformances = forecastedKpiMetricsPerformances,
            ForecastedWeightedScore = forecastedWeightedScore
        };
    }

    private async Task<EvaluationSession> GetSessionWithValidation(int id, CancellationToken cancellationToken)
    {
        var evaluationSession = await evaluationSessionsRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (evaluationSession == null)
            throw new InvalidOperationException($"Evaluation session with Id={id} not found.");
        if (evaluationSession.EvaluationFinishedDate == null)
            throw new InvalidOperationException($"Evaluation session with Id={id} is not finished yet.");

        return evaluationSession;
    }

    private async Task<List<EvaluationSession>> GetAllFinishedSessionsForEmployee(int employeeId, CancellationToken cancellationToken)
    {
        return (await evaluationSessionsRepository.GetAllWithDetailsAsync(employeeId, true, cancellationToken))
            .OrderBy(s => s.EvaluationFinishedDate)
            .ToList();
    }

    private static List<KpiMetricPerformance> CalculateAverageScoreForEachRoleKpi(List<Evaluation> evaluations)
    {
        return [.. evaluations
            .GroupBy(e => new { e.KpiId, e.RoleId })
            .Select(g =>
            {
                var roleKpi = g.FirstOrDefault(e => e.KpiId == g.Key.KpiId && e.RoleId == g.Key.RoleId)!.RoleKpi!;
                return new KpiMetricPerformance
                {
                    KpiId = roleKpi.KpiId,
                    KpiName = roleKpi.KpiMetric!.Name,
                    KpiWeight = roleKpi.Weight,
                    AverageScore = g.Average(e => (decimal)e.Score)
                };
            })];
    }

    private async Task<List<KpiMetricPerformance>> ForecastNextPeriodPerformance(List<EvaluationSession> evaluationSessions, CancellationToken cancellationToken)
    {
        var allEvaluations = await evaluationsRepository.GetAllWithDetailsAsync(cancellationToken);

        var evaluationSessionsWithKpiMetricsPerformances = evaluationSessions.Select(evaluationSession =>
        {
            var evaluations = allEvaluations.Where(x => x.EvaluationSessionId == evaluationSession.Id).ToList();
            var avarageScoreForEachRoleKpi = CalculateAverageScoreForEachRoleKpi(evaluations);

            return new EvaluationSessionWithKpiMetricsPerformances
            {
                EvaluationSession = evaluationSession,
                KpiMetricPerformances = avarageScoreForEachRoleKpi,
            };
        });

        // Groups of evaluated KPI metrics for all sessions grouped by KpiId
        var kpiMetricsPerformacesGrouped = evaluationSessionsWithKpiMetricsPerformances
            .SelectMany(x =>
            {
                return x.KpiMetricPerformances;
            })
            .GroupBy(e => e.KpiId)
            .ToDictionary(keySelector: e => e.Key, elementSelector: e => e.ToList());

        var alpha = 0.7M;
        var beta = 0.3M;
        var forecastedKpiMetricsPerformances = kpiMetricsPerformacesGrouped.Select(x =>
        {
            var kpiMetricPerformances = x.Value;
            var firstKpiMetricPerformanceScore = kpiMetricPerformances.First().AverageScore;
            var secondKpiMetricPerformanceScore = kpiMetricPerformances.Skip(1).First().AverageScore;

            var level = firstKpiMetricPerformanceScore;
            var trend = secondKpiMetricPerformanceScore - firstKpiMetricPerformanceScore;

            foreach (var kpiMetricPerformance in kpiMetricPerformances.Skip(2))
            {
                var level2 = alpha * kpiMetricPerformance.AverageScore + (1 - alpha) * (level + trend);
                var trend2 = beta * (level2 - level) + (1 - beta) * trend;
                level = level2;
                trend = trend2;
            }

            var forecastedScore = level + trend;
            return new KpiMetricPerformance
            {
                KpiId = x.Key,
                KpiName = kpiMetricPerformances.First().KpiName,
                KpiWeight = kpiMetricPerformances.First().KpiWeight,
                AverageScore = forecastedScore
            };
        });

        return [.. forecastedKpiMetricsPerformances];
    }
}
