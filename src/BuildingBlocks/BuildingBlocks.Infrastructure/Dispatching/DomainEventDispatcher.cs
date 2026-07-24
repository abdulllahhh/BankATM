using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Infrastructure.Dispatching;

/// <summary>
/// Publishes domain events through MediatR and enqueues them into the outbox.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
