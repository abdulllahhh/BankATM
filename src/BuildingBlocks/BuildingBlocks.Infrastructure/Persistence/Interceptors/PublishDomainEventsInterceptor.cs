using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Collects domain events from all tracked aggregates after a successful
/// <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/>
/// and dispatches them through <see cref="IDomainEventDispatcher"/>.
///
/// Domain events are cleared from the aggregates after dispatch to prevent
/// re-dispatching on subsequent saves.
/// </summary>
public sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public PublishDomainEventsInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var aggregates = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        if (aggregates.Count == 0)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
