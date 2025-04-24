using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;

public class TeamViewModelWithEmployees
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }

    public List<EmployeePartialViewModel> Employees { get; set; } = [];

    public static TeamViewModelWithEmployees MapFromDbModel(Team model)
    {
        return new TeamViewModelWithEmployees
        {
            Id = model.Id,
            Name = model.Name,
            TeamLeadId = model.TeamLeadId,

            Employees = model.Employees?.Select(EmployeePartialViewModel.MapFromDbModel).ToList() ?? []
        };
    }
}
