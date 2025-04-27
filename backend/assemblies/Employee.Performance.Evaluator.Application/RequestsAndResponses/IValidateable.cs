namespace Employee.Performance.Evaluator.Application.RequestsAndResponses;

public interface IValidateable
{
    /// <summary>
    /// Validates the request and throws an exception if validation fails.
    /// </summary>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    void Validate();
}
