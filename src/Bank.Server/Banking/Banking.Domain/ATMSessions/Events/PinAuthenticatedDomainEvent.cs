using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATMSessions.Events;

public sealed record PinAuthenticatedDomainEvent(
    ValueObjects.SessionId SessionId,
    DateTime AuthenticatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
