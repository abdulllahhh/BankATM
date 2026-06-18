using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.BaseEntity
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent>
            DomainEvents
        { get; }

        void ClearDomainEvents();
    }
}
