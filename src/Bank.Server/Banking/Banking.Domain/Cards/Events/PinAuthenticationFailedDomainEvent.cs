using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record PinAuthenticationFailedDomainEvent(
    ValueObjects.CardNumber CardNumber,
    int FailedAttempts,
    DateTime AttemptedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
