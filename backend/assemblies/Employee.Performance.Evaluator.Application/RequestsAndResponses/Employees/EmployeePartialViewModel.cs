using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;

public class EmployeePartialViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte[]? Avatar { get; set; }

    public int? TeamId { get; set; }
    public int UserId { get; set; }

    public UserPartialViewModel User { get; set; } = new();

    public static EmployeePartialViewModel MapFromDbModel(EmployeeEntity model)
    {
        if (model.User == null)
            throw new InvalidOperationException("User cannot be null");

        return new EmployeePartialViewModel
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Avatar = model.Avatar,
            TeamId = model.TeamId,
            UserId = model.UserId,
            User = UserPartialViewModel.MapFromDbModel(model.User)
        };
    }
}
