using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Infrastructure.Events
{
    public sealed class DomainEventsAccessor : IDomainEventsAccessor
    {
        private readonly DbContext _context;

        public DomainEventsAccessor(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IReadOnlyCollection<IDomainEvent> ExtractDomainEvents()
        {
            var aggregateRoots = _context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(entry => entry.Entity)
                .ToList();

            var domainEvents = aggregateRoots
                .SelectMany(root => root.DomainEvents)
                .ToList();

            foreach (var aggregateRoot in aggregateRoots)
            {
                aggregateRoot.ClearDomainEvents();
            }

            return domainEvents;
        }
    }
}
