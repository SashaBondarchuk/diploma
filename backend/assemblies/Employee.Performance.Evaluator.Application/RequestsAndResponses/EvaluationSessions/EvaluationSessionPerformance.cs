using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;

public class EvaluationSessionPerformance
{
    public decimal WeightedScore { get; set; }

    public List<KpiMetricPerformance> KpiMetricPerformances { get; set; } = [];
}

public class EvaluationSessionWithKpiMetricsPerformances
{
    public EvaluationSession EvaluationSession { get; set; } = new();

    public List<KpiMetricPerformance> KpiMetricPerformances { get; set; } = [];
}

public class KpiMetricPerformance
{
    public int KpiId { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public decimal KpiWeight { get; set; }
    public decimal AverageScore { get; set; }
}