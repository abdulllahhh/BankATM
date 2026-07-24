using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred while processing request {RequestName}",
                typeof(TRequest).Name);

            throw;
        }
    }
}
