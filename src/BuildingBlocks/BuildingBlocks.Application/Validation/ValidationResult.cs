namespace BuildingBlocks.Application.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyCollection<ValidationError> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyCollection<ValidationError> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Success() => new(true, []);

    public static ValidationResult Failure(IReadOnlyCollection<ValidationError> errors) => new(false, errors);
}
