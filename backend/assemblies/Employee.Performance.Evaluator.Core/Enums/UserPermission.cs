using System.ComponentModel.DataAnnotations;

namespace Employee.Performance.Evaluator.Core.Enums;

public enum UserPermission
{
    [Display(Name = "base")]
    Base = 1,

    [Display(Name = "manage_roles")]
    ManageRoles = 2,

    [Display(Name = "manage_kpis")]
    ManageKpis = 3,

    [Display(Name = "manage_employees")]
    ManageEmployees = 4,

    [Display(Name = "manage_teams")]
    ManageTeams = 5,

    [Display(Name = "manage_evaluations")]
    ManageEvaluations = 6,

    [Display(Name = "create_recommendations")]
    CreateRecommendations = 7,

    [Display(Name = "view_all_evaluations")]
    ViewAllEvaluations = 8,

    [Display(Name = "evaluate_team_members_lead")]
    EvaluateTeamMembersLead = 9,

    [Display(Name = "evaluate_team_members")]
    EvaluateTeamMembers = 10,
}
