namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;

public class AddEvaluationSessionRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset EndDate { get; set; }
    public int EmployeeId { get; set; }
}
