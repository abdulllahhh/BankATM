using BuildingBlocks.Domain.Events;

namespace Bank.Server.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}