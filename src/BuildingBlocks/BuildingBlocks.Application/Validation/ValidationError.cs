namespace BuildingBlocks.Application.Validation;

public sealed record ValidationError(string PropertyName, string ErrorMessage, string? Code = null);
