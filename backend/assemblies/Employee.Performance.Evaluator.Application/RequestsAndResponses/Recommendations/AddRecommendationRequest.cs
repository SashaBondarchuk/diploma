namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;

public class AddRecommendationRequest
{
    public int EmployeeId { get; set; }
    public string RecommendationText { get; set; } = string.Empty;
    public bool IsVisibleToEmployee { get; set; }
}
