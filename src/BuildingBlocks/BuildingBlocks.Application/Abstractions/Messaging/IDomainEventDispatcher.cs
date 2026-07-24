using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Application.Abstractions.Messaging;

/// <summary>
/// Dispatches domain events collected from aggregate roots to their registered handlers.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the provided domain events.
    /// </summary>
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
