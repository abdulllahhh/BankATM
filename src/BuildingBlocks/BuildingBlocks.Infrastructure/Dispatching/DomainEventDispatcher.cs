using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Events;
using MediatR;

namespace BuildingBlocks.Infrastructure.Dispatching;

/// <summary>
/// Dispatches domain events by publishing each event through MediatR's
/// <see cref="IPublisher"/>. This allows domain event handlers registered
/// as <see cref="INotificationHandler{TNotification}"/> to process events
/// after the aggregate has been persisted.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
