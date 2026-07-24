using System.Reflection;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Results;
using MediatR;

namespace BuildingBlocks.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsCommand())
            return await next();

        var response = await next();

        if (IsSuccess(response))
        {
            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (saveResult.IsFailure)
                return CreateFailureResult(saveResult.Error);
        }

        return response;
    }

    private static bool IsCommand()
    {
        var type = typeof(TRequest);
        return type.Name.EndsWith("Command") ||
               type.GetInterfaces().Any(i =>
                   i == typeof(ICommand) ||
                   (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));
    }

    private static bool IsSuccess(TResponse response)
    {
        if (response is Result result)
            return result.IsSuccess;

        var prop = typeof(TResponse).GetProperty("IsSuccess");
        return prop is null || (bool)prop.GetValue(response)!;
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
