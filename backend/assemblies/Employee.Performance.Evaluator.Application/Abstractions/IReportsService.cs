using Employee.Performance.Evaluator.Application.RequestsAndResponses.Reporting;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IReportsService
{
    MemoryStream GeneratePdfReport(EvaluationSession session,
        List<Evaluation> evaluations,
        List<KpiHistoryRow> kpiTable,
        List<string> sessionNames,
        bool canForecast,
        decimal? forecastedWeightedScore);
}
