namespace BuildingBlocks.Application.Validation;

/// <summary>
/// Validates instances of <typeparamref name="T"/>.
/// </summary>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the specified instance and returns a <see cref="ValidationResult"/>.
    /// </summary>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
