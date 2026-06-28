using Bank.Server.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Application.Common.Events;

public sealed class DomainEventsAccessor
{
    private readonly DbContext _context;

    public DomainEventsAccessor(DbContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<IDomainEvent> GetAllDomainEvents()
    {
        return _context.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }

    public void ClearAllDomainEvents()
    {
        foreach (var entity in _context.ChangeTracker
                     .Entries<AggregateRoot<Guid>>())
        {
            entity.Entity.ClearDomainEvents();
        }
    }
}