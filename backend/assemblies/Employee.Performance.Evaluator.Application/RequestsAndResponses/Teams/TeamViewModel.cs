using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;

public class TeamViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? TeamLeadId { get; set; }
    public byte[]? TeamLeadAvatar { get; set; }
    public string? TeamLeadFirstName { get; set; }
    public string? TeamLeadLastName { get; set; }

    public static TeamViewModel MapFromDbModel(Team team)
    {
        return new TeamViewModel
        {
            Id = team.Id,
            Name = team.Name,
            TeamLeadId = team.TeamLead?.UserId,
            TeamLeadAvatar = team.TeamLead?.Avatar,
            TeamLeadFirstName = team.TeamLead?.FirstName,
            TeamLeadLastName = team.TeamLead?.LastName
        };
    }
}
