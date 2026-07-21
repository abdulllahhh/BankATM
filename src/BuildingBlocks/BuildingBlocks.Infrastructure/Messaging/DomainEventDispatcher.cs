using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Infrastructure.Events;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Infrastructure.Messaging
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IDomainEventsAccessor _accessor;
        private readonly IMediator _mediator;

        public DomainEventDispatcher(
            IDomainEventsAccessor accessor,
            IMediator mediator)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task DispatchAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEvents = _accessor.ExtractDomainEvents();
                if (domainEvents == null || !domainEvents.Any())
                {
                    break;
                }

                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }
    }
}
