using Employee.Performance.Evaluator.Application.RequestsAndResponses.EmployeeClass;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;

public class EvaluationSessionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset? EvaluationFinishedDate { get; set; }
    public int EmployeeId { get; set; }
    public int? ClassId { get; set; }
    public decimal? WeightedScore { get; set; }
    public bool IsReportAvailable { get; set; }

    public EmployeePartialViewModel? Employee { get; set; }
    public EmployeeClassViewModel? Class { get; set; }

    public static EvaluationSessionViewModel MapFromDbModel(EvaluationSession model)
    {
        return new EvaluationSessionViewModel
        {
            Id = model.Id,
            Name = model.Name,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            EvaluationFinishedDate = model.EvaluationFinishedDate,
            EmployeeId = model.EmployeeId,
            ClassId = model.ClassId,
            WeightedScore = model.WeightedScore,
            IsReportAvailable = model.ReportFile != null,
            Employee = EmployeePartialViewModel.MapFromDbModel(model.Employee!),
            Class = model.Class != null ? EmployeeClassViewModel.MapFromDbModel(model.Class) : null
        };
    }
}
