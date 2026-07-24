using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Aggregates;

public sealed class CashDispenser : AggregateRoot<Guid>
{
    public Guid ATMId { get; private set; }
    public decimal Denomination { get; private set; }
    public int Count { get; private set; }

    private CashDispenser() { }

    public CashDispenser(Guid id, Guid atmId, decimal denomination, int count)
        : base(id)
    {
        ATMId = atmId;
        Denomination = denomination;
        Count = count;
    }
}
