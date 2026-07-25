using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATMSessions.Events;

public sealed record SessionStartedDomainEvent(
    ValueObjects.SessionId SessionId,
    ValueObjects.ATMId ATMId,
    DateTime StartedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
