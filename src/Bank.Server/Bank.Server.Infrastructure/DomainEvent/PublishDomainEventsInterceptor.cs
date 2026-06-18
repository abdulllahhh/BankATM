using Bank.Server.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Infrastructure.DomainEvent
{
    public sealed class PublishDomainEventsInterceptor
        : SaveChangesInterceptor
    {
        private readonly IDomainEventDispatcher
            _dispatcher;

        public PublishDomainEventsInterceptor(
            IDomainEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public override async ValueTask<
            int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
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
