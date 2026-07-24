namespace BuildingBlocks.Application.Abstractions.Messaging;

/// <summary>
/// Carries the correlation identifier for the current request, enabling cross-service tracing.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// The correlation identifier for the current request scope.
    /// </summary>
    string CorrelationId { get; }
}
