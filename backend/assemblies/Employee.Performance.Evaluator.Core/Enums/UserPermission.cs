using System.ComponentModel.DataAnnotations;

namespace Employee.Performance.Evaluator.Core.Enums;

public enum UserPermission
{
    [Display(Name = "assign_roles")]
    AssignRoles = 1,

    [Display(Name = "edit_employee")]
    EditEmployee = 2,

    [Display(Name = "manage_kpi")]
    ManageKPI = 3,

    [Display(Name = "create_teams")]
    CreateTeams = 4,

    [Display(Name = "view_reports")]
    ViewReports = 5,

    [Display(Name = "create_recommendations")]
    CreateRecommendations = 6,

    [Display(Name = "evaluate_employees")]
    EvaluateEmployees = 7,

    [Display(Name = "comment_evaluation")]
    CommentEvaluation = 8,

    [Display(Name = "view_own_reports")]
    ViewOwnReports = 9,

    [Display(Name = "download_reports")]
    DownloadReports = 10,

    [Display(Name = "peer_evaluation")]
    PeerEvaluation = 11
}
