using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IUserGetter
{
    /// <summary>
    /// Returns current userId or null if no userId is presented
    /// </summary>
    int? CurrentUserId { get; }

    /// <summary>
    /// Returns current user or null if no user is presented
    /// </summary>
    User? CurrentUser { get; }

    /// <summary>
    /// Throws exception if not userId is presented
    /// </summary>
    /// <returns></returns>
    int GetCurrentUserIdOrThrow();

    /// <summary>
    /// Throws exception if not user is presented
    /// </summary>
    /// <returns></returns>
    User GetCurrentUserOrThrow();
}