namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Evaluations;

public class AddEvaluationRequest
{
    public int Score { get; set; }
    public string? Comment { get; set; }
    public int EvaluationSessionId { get; set; }
    public int KpiId { get; set; }
    public int RoleId { get; set; }
}
