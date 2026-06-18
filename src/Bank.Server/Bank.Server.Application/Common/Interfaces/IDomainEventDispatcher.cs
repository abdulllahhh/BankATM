using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Application.Common.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(
            IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken);
    }
}
