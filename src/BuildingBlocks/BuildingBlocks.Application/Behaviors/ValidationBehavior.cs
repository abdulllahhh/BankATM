using BuildingBlocks.Application.Results;
using BuildingBlocks.Application.Validation;
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
        var failures = new List<ValidationError>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Count > 0)
        {
            var error = new Error("VALIDATION.FAILED", "One or more validation errors occurred.");
            return CreateFailureResult(error);
        }

        return await next();
    }

    private static TResponse CreateFailureResult(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var method = typeof(Result)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Failure" && m.IsGenericMethodDefinition);

        return (TResponse)(object)method
            .MakeGenericMethod(valueType)
            .Invoke(null, [error])!;
    }
}
