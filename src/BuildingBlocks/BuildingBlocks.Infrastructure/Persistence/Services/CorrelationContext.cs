using BuildingBlocks.Application.Abstractions.Messaging;

namespace BuildingBlocks.Infrastructure.Persistence.Services;

public class CorrelationContext : ICorrelationContext
{
    public string CorrelationId => string.Empty;
}
