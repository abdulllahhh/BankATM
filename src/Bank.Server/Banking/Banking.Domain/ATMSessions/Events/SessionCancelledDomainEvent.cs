using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATMSessions.Events;

public sealed record SessionCancelledDomainEvent(
    ValueObjects.SessionId SessionId,
    string Reason,
    DateTime CancelledAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
