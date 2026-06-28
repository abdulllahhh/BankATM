using Bank.Server.Domain.Common;
using BuildingBlocks.SharedKernel.DomainEvents;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Infrastructure.DomainEvent
{
    internal static class DomainEventsExtractor
    {
        public static List<IDomainEvent>
            Extract(DbContext context)
        {
            var domainEvents =
                context.ChangeTracker
                    .Entries<AggregateRoot<Guid>>()
                    .Select(entry => entry.Entity)
                    .SelectMany(entity =>
                    {
                        var events =
                            entity.DomainEvents.ToList();

                        entity.ClearDomainEvents();

                        return events;
                    })
                    .ToList();

            return domainEvents;
        }
    }
}
