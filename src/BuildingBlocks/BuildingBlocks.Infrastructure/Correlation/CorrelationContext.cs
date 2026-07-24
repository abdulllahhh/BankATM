using BuildingBlocks.Application.Abstractions.Messaging;

namespace BuildingBlocks.Infrastructure.Correlation;

/// <summary>
/// Holds the correlation identifier for the current request scope.
/// </summary>
public sealed class CorrelationContext : ICorrelationContext
{
    public string CorrelationId => string.Empty;
}
