using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.SharedKernel.DomainEvents
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
