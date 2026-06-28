using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.DomainEvents
{
    public class CardMarkedAsStolenDomainEvent : INotification
    {
        public string CardId { get; set; }
    }
}
