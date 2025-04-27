using Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;

public class EmployeeViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset BirthDate { get; set; }
    public DateTimeOffset HireDate { get; set; }
    public int? TeamId { get; set; }
    public int UserId { get; set; }
    public byte[]? Avatar { get; set; }

    public TeamViewModel? Team { get; set; } = new();
    public UserViewModel User { get; set; } = new();

    public static EmployeeViewModel MapFromDbModel(EmployeeEntity model)
    {
        if (model.User == null)
            throw new InvalidOperationException("User cannot be null");

        return new EmployeeViewModel
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            BirthDate = model.BirthDate,
            HireDate = model.HireDate,
            TeamId = model.TeamId,
            UserId = model.UserId,
            Avatar = model.Avatar,
            Team = model.Team != null ? TeamViewModel.MapFromDbModel(model.Team) : null,
            User = UserViewModel.MapFromDbModel(model.User)
        };
    }
}
