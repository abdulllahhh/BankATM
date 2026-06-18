using BuildingBlocks.SharedKernel.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.DomainEvents
{
    public sealed record CardIssuedDomainEvent(
        Guid CardId)
        : DomainEvent;
}
