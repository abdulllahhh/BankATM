using Bank.Server.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Events;
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
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
        }
    }
}
