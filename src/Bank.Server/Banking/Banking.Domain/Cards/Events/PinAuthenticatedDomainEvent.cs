using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record PinAuthenticatedDomainEvent(
    ValueObjects.CardNumber CardNumber,
    DateTime AuthenticatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
