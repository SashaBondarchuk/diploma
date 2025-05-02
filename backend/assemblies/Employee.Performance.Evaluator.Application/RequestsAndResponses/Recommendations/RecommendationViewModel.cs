using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;

public class RecommendationViewModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string RecommendationText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsVisibleToEmployee { get; set; }

    public EmployeePartialViewModel? Employee { get; set; }

    public static RecommendationViewModel MapFromDbModel(Recommendation recommendation)
    {
        return new RecommendationViewModel
        {
            Id = recommendation.Id,
            EmployeeId = recommendation.EmployeeId,
            RecommendationText = recommendation.RecommendationText,
            CreatedAt = recommendation.CreatedAt,
            IsVisibleToEmployee = recommendation.IsVisibleToEmployee,
            Employee = EmployeePartialViewModel.MapFromDbModel(recommendation.Employee!)
        };
    }
}
