using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.AccountContext.DomainEvents
{
    public sealed record AccountCreatedDomainEvent(
        Guid AccountId)
        : DomainEvent;
}
