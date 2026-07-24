using BuildingBlocks.Application.Results;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection;

namespace BuildingBlocks.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
            return await next();

        var distinct = failures
            .DistinctBy(f => new { f.PropertyName, f.ErrorMessage })
            .ToList();

        var error = new Error(
            "VALIDATION.FAILED",
            $"Validation failed: {string.Join("; ", distinct.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"))}");

        return CreateFailureResult(error);
    }

    private static TResponse CreateFailureResult(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var method = typeof(Result)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Failure" && m.IsGenericMethodDefinition);

        return (TResponse)(object)method
            .MakeGenericMethod(valueType)
            .Invoke(null, [error])!;
    }
}
