using Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Reporting;

public class KpiHistoryAndForecastResult
{
    public List<KpiHistoryRow> KpiTable { get; set; } = new();
    public List<string> SessionNames { get; set; } = new();
    public bool CanForecast { get; set; }
    public List<KpiMetricPerformance> ForecastedKpiMetricsPerformances { get; set; } = new();
    public decimal? ForecastedWeightedScore { get; set; }
}