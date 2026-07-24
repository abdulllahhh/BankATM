using BuildingBlocks.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        _logger.LogInformation(
            "Processing request {RequestName} [Correlation: {CorrelationId}]",
            typeof(TRequest).Name,
            correlationId);

        var response = await next();

        if (IsSuccess(response))
        {
            _logger.LogInformation(
                "Request {RequestName} completed successfully [Correlation: {CorrelationId}]",
                typeof(TRequest).Name,
                correlationId);
        }
        else
        {
            _logger.LogWarning(
                "Request {RequestName} failed [Correlation: {CorrelationId}]",
                typeof(TRequest).Name,
                correlationId);
        }

        return response;
    }

    private static bool IsSuccess(TResponse response)
    {
        if (response is Result result)
            return result.IsSuccess;

        var isSuccessProperty = typeof(TResponse).GetProperty("IsSuccess");
        return isSuccessProperty is null || (bool)isSuccessProperty.GetValue(response)!;
    }
}
