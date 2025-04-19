namespace Employee.Performance.Evaluator.Core.Exceptions;

public class InvalidTokenException(string? token) : Exception($"Invalid token: {token}")
{
}
