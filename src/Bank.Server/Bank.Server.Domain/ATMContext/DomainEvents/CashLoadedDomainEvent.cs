using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.ATMContext.DomainEvents
{
    public sealed record CashLoadedDomainEvent(
        Guid AtmId,
        decimal Amount)
        : DomainEvent;
}
