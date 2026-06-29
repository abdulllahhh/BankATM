using Bank.Server.Application.Abstractions.Messaging;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bank.Server.Infrastructure.Persistence.Interceptors;

public sealed class DomainEventInterceptor
    : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventInterceptor(
        IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
            return result;

        var domainEvents = dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var entity in dbContext.ChangeTracker
            .Entries<AggregateRoot<Guid>>())
        {
            entity.Entity.ClearDomainEvents();
        }

        await _dispatcher.DispatchAsync(cancellationToken);

        return result;
    }
}