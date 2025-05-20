namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Reporting;

public class KpiHistoryRow
{
    public int KpiId { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public List<decimal> Scores { get; set; } = [];
}
