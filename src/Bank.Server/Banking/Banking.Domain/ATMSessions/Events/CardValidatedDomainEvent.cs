using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATMSessions.Events;

public sealed record CardValidatedDomainEvent(
    ValueObjects.SessionId SessionId,
    ValueObjects.CardId CardId,
    DateTime ValidatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
