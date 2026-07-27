using BuildingBlocks.Domain.Events;

namespace Banking.Domain.Cards.Events;

public sealed record CardBlockedDomainEvent(
    Guid CardId,
    string Reason)
    : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
