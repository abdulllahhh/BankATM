using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.Entities;

public sealed class CashInventory : Entity<Guid>
{
    public decimal Denomination { get; private set; }
    public int Count { get; private set; }
    public int ReservedCount { get; private set; }

    public int AvailableCount => Count - ReservedCount;

    private CashInventory() { }

    private CashInventory(Guid id, decimal denomination, int count) : base(id)
    {
        Denomination = denomination;
        Count = count;
        ReservedCount = 0;
    }

    public static CashInventory Create(decimal denomination, int count)
    {
        if (denomination <= 0)
        {
            throw new DomainException("Denomination must be greater than zero.");
        }

        if (count < 0)
        {
            throw new DomainException("Count cannot be negative.");
        }

        return new CashInventory(Guid.NewGuid(), denomination, count);
    }

    public bool CanReserve(int quantity)
    {
        return quantity > 0 && AvailableCount >= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Reserve quantity must be greater than zero.");
        }

        if (!CanReserve(quantity))
        {
            throw new DomainException("Insufficient available cash to reserve.");
        }

        ReservedCount += quantity;
    }

    public void Dispense(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Dispense quantity must be greater than zero.");
        }

        if (ReservedCount < quantity)
        {
            throw new DomainException("Cannot dispense more than the reserved quantity.");
        }

        Count -= quantity;
        ReservedCount -= quantity;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Release quantity must be greater than zero.");
        }

        if (ReservedCount < quantity)
        {
            throw new DomainException("Cannot release more than the reserved quantity.");
        }

        ReservedCount -= quantity;
    }

    public void Replenish(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Replenish quantity must be greater than zero.");
        }

        Count += quantity;
    }
}
