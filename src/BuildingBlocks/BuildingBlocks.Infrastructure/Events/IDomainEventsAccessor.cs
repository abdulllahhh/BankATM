using BuildingBlocks.Domain.Events;
using System.Collections.Generic;

namespace BuildingBlocks.Infrastructure.Events
{
    public interface IDomainEventsAccessor
    {
        IReadOnlyCollection<IDomainEvent> ExtractDomainEvents();
    }
}
