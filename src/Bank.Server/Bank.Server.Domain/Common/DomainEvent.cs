using BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.SharedKernel.DomainEvents
{
    public abstract record DomainEvent : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; init; } =
            DateTime.UtcNow;
    }
}
