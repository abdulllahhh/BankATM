using Banking.Domain.ATM.Enums;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.Entities;

public sealed class CashCassette : Entity<CassetteId>
{
    private const decimal LowThresholdRatio = 0.2m;

    public Denomination Denomination { get; private set; } = null!;
    public int Capacity { get; private set; }
    public int CurrentCount { get; private set; }
    public int ReservedCount { get; private set; }

    public int AvailableCount => CurrentCount - ReservedCount;

    public CassetteStatus Status
    {
        get
        {
            if (CurrentCount == 0) return CassetteStatus.Empty;
            if (CurrentCount <= Capacity * LowThresholdRatio) return CassetteStatus.Low;
            return CassetteStatus.Active;
        }
    }

    private CashCassette() { }

    private CashCassette(CassetteId id, Denomination denomination, int capacity, int initialCount) : base(id)
    {
        Denomination = denomination;
        Capacity = capacity;
        CurrentCount = initialCount;
        ReservedCount = 0;
    }

    public static CashCassette Load(CassetteId id, Denomination denomination, int capacity, int initialCount)
    {
        if (capacity <= 0)
        {
            throw new DomainException("Cassette capacity must be greater than zero.");
        }

        if (initialCount < 0)
        {
            throw new DomainException("Initial count cannot be negative.");
        }

        if (initialCount > capacity)
        {
            throw new DomainException("Initial count cannot exceed cassette capacity.");
        }

        return new CashCassette(id, denomination, capacity, initialCount);
    }

    public static CashCassette Create(Denomination denomination, int capacity)
    {
        return Load(CassetteId.New(), denomination, capacity, 0);
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

        CurrentCount -= quantity;
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

        if (CurrentCount + quantity > Capacity)
        {
            throw new DomainException("Replenishment would exceed cassette capacity.");
        }

        CurrentCount += quantity;
    }

    public bool HasCash => CurrentCount > 0;
}
