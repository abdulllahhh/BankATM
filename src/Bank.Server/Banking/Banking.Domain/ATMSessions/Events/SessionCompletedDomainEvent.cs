using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATMSessions.Events;

public sealed record SessionCompletedDomainEvent(
    ValueObjects.SessionId SessionId,
    ValueObjects.TransactionNumber TransactionNumber,
    DateTime CompletedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
