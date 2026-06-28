using Bank.Server.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bank.Server.Infrastructure.DomainEvent
{
    public sealed class PublishDomainEventsInterceptor(
        IDomainEventDispatcher dispatcher)
                : SaveChangesInterceptor
    {
        private readonly IDomainEventDispatcher
            _dispatcher = dispatcher;

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
                return result;

            var domainEvents =
                DomainEventsExtractor.Extract(
                    eventData.Context);

            await _dispatcher.DispatchAsync(
                domainEvents,
                cancellationToken);

            return result;
        }
    }
}
