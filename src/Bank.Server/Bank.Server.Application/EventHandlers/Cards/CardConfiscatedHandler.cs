using Bank.Server.Domain.CardContext.DomainEvents;
using MediatR;

namespace Bank.Server.Application.EventHandlers.Cards;

public sealed class CardConfiscatedHandler
    : INotificationHandler<CardMarkedAsStolenDomainEvent>
{
    public Task Handle(
        CardMarkedAsStolenDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Example reactions:
        // - log security event
        // - notify fraud system
        // - block card in cache layer

        Console.WriteLine($"SECURITY ALERT: Card {notification.CardId} confiscated");

        return Task.CompletedTask;
    }
}