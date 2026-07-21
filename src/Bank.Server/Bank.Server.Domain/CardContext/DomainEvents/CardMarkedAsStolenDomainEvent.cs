using BuildingBlocks.SharedKernel.DomainEvents;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.DomainEvents
{
    public sealed record CardMarkedAsStolenDomainEvent(
        string CardId)
        : DomainEvent;
}
