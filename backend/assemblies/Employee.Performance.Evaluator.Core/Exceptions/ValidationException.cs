namespace Employee.Performance.Evaluator.Core.Exceptions;

public class ValidationException(string message) : Exception(message)
{
}