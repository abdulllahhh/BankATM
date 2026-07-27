using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record PinAuthenticationFailedDomainEvent(
    Guid CardId,
    int FailedAttempts)
    : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
