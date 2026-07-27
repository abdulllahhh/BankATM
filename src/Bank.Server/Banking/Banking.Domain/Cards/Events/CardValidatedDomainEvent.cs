using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record CardValidatedDomainEvent(
    Guid CardId)
    : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
