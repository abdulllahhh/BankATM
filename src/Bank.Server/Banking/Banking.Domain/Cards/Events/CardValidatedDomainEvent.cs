using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record CardValidatedDomainEvent(
    ValueObjects.CardNumber CardNumber,
    DateTime ValidatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
