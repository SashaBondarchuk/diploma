using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;

public class UserViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public Role Role { get; set; } = new();

    public static UserViewModel MapFromDbModel(User user)
    {
        if (user.Role == null)
            throw new InvalidOperationException("Role cannot be null");

        return new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            RoleId = user.RoleId,
            Role = user.Role
        };
    }
}
