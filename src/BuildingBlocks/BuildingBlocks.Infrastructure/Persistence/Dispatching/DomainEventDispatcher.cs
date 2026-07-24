using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Infrastructure.Persistence.Dispatching;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
