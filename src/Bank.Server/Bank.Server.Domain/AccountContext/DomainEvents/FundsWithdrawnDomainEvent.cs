using BuildingBlocks.SharedKernel.DomainEvents;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.AccountContext.DomainEvents
{
    public sealed record FundsWithdrawnDomainEvent(
        Guid AccountId,
        Guid AtmId,
        decimal Amount,
        string Currency,
        Guid TransactionId)
        : DomainEvent, INotification;
}
