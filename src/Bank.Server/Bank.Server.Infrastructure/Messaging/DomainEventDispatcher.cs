using Bank.Server.Application.Abstractions.Messaging;
using Bank.Server.Infrastructure.Persistence;
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

        public async Task DispatchAsync(CancellationToken ct)
        {
            //var events = _accessor.GetAllDomainEvents();

            //_accessor.ClearAllDomainEvents();

            //foreach (var domainEvent in events)
            //{
            //    await _publisher.Publish(domainEvent, ct);
            //}
        }
    }
}