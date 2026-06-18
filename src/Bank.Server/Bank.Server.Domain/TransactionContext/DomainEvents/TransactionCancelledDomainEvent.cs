using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.TransactionContext.DomainEvents
{
    public sealed record TransactionCancelledDomainEvent(
        Guid TransactionId)
        : DomainEvent;
}
