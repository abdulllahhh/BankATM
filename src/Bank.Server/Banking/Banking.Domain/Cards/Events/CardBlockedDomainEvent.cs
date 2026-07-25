using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record CardBlockedDomainEvent(
    ValueObjects.CardNumber CardNumber,
    string Reason,
    DateTime BlockedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
