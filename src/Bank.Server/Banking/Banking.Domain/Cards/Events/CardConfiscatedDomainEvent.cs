using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record CardConfiscatedDomainEvent(
    ValueObjects.CardNumber CardNumber,
    string Reason,
    DateTime ConfiscatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
