namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;

public class GetTeamViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }

    //public Employee? TeamLead { get; set; }
}
