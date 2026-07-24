using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Domain.Common
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent>
            DomainEvents
        { get; }

        void ClearDomainEvents();
    }
}
