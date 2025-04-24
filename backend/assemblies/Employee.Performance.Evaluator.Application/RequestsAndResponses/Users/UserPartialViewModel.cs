using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;

public class UserPartialViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;

    public static UserPartialViewModel MapFromDbModel(User user)
    {
        if (user.Role == null)
            throw new InvalidOperationException("Role cannot be null");

        return new UserPartialViewModel
        {
            Id = user.Id,
            Email = user.Email,
            RoleName = user.Role.RoleName
        };
    }
}
