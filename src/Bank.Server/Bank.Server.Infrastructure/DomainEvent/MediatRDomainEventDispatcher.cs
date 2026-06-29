using Bank.Server.Application.Abstractions.Messaging;
using BuildingBlocks.Domain;
using MediatR;


namespace Bank.Server.Infrastructure.DomainEvent
{
    public sealed class MediatRDomainEventDispatcher
        : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public MediatRDomainEventDispatcher(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(
            //IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            //foreach (var domainEvent in domainEvents)
            //{
            //    await _mediator.Publish(
            //        domainEvent,
            //        cancellationToken);
            //}
        }
    }
}
