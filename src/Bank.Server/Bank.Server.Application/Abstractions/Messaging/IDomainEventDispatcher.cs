using Bank.Server.Domain.Common;
using BuildingBlocks.SharedKernel.DomainEvents;

namespace Bank.Server.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken);
}