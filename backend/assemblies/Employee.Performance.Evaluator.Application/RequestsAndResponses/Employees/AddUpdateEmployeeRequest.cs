using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;

public class AddUpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset BirthDate { get; set; }
    public int TeamId { get; set; }
    public bool IsTeamLead { get; set; }
    public int UserId { get; set; }
    public string? Avatar { get; set; }

    public int RoleId { get; set; }

    public static EmployeeEntity ToDbModel(AddUpdateEmployeeRequest request)
    {
        return new EmployeeEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            BirthDate = request.BirthDate,
            TeamId = request.TeamId,
            UserId = request.UserId,
            Avatar = string.IsNullOrEmpty(request.Avatar)
                ? null
                : Convert.FromBase64String(
                    request.Avatar.Split(',').Last()
                )
        };
    }
}
