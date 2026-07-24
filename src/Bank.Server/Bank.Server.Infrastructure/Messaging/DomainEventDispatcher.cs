using Bank.Server.Application.Abstractions.Messaging;
using Bank.Server.Infrastructure.Persistence;
using BuildingBlocks.Domain.Events;
using MediatR;

namespace Bank.Server.Infrastructure.Messaging
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        //private readonly DomainEventsAccessor _accessor;
        //private readonly IPublisher _publisher;

        public DomainEventDispatcher(
            //DomainEventsAccessor accessor,
            //IPublisher publisher
            )
        {
            //_accessor = accessor;
            //_publisher = publisher;
        }

        public async Task DispatchAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default)
        {
        }
    }
}