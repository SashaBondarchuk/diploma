using EmployeeClassEntity = Employee.Performance.Evaluator.Core.Entities.EmployeeClass;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.EmployeeClass;

public class EmployeeClassViewModel
{
    public int Id { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] RecommendedActions { get; set; } = [];
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }

    public static EmployeeClassViewModel MapFromDbModel(EmployeeClassEntity model)
    {
        return new EmployeeClassViewModel
        {
            Id = model.Id,
            ClassName = model.ClassName,
            Description = model.Description,
            RecommendedActions = model.RecommendedActions,
            MinScore = model.MinScore,
            MaxScore = model.MaxScore
        };
    }
}
