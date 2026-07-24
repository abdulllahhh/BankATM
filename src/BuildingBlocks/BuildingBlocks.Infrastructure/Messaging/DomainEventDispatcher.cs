using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Infrastructure.Messaging
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IDomainEventsAccessor _accessor;
        private readonly DbContext _dbContext;
        private readonly IMediator _mediator;

        public DomainEventDispatcher(
            IDomainEventsAccessor accessor,
            DbContext dbContext,
            IMediator mediator)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task DispatchAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                EnqueueOutboxMessage(domainEvent);

                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        private void EnqueueOutboxMessage(IDomainEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            var content = JsonSerializer.Serialize(domainEvent, eventType);
            var type = eventType.FullName ?? eventType.Name;

            var outboxMessage = OutboxMessage.FromDomainEvent(
                domainEvent.EventId,
                type,
                content,
                domainEvent.OccurredOnUtc);

            _dbContext.Set<OutboxMessage>().Add(outboxMessage);
        }
    }
}
