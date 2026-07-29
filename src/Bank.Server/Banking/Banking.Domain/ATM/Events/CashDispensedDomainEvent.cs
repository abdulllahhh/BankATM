using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.Events;

public sealed record CashDispensedDomainEvent(
    ATMId ATMId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
