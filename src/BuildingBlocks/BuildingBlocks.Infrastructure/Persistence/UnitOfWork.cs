using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Result = BuildingBlocks.Application.Results.Result;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Commits tracked changes and dispatches domain events within a single business transaction.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UnitOfWork(
        DbContext dbContext,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _dbContext = dbContext;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        if (domainEvents.Count > 0)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries<IAggregateRoot>())
            {
                entry.Entity.ClearDomainEvents();
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (domainEvents.Count > 0)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return Result.Success();
    }
}
