using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Events;

namespace Banking.Domain.ATM.Events;

public sealed record MaintenanceCompletedDomainEvent(ATMId ATMId) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
