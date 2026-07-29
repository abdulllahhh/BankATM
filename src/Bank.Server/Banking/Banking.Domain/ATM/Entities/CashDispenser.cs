using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.Entities;

public sealed class CashDispenser : Entity<Guid>
{
    private readonly List<CashCassette> _cassettes = new();

    public IReadOnlyCollection<CashCassette> Cassettes => _cassettes.AsReadOnly();

    private CashDispenser() { }

    private CashDispenser(Guid id) : base(id) { }

    public static CashDispenser Create()
    {
        return new CashDispenser(Guid.NewGuid());
    }

    public Result AddCassette(Denomination denomination, int capacity)
    {
        if (_cassettes.Any(c => c.Denomination.Equals(denomination)))
        {
            return Result.Failure("A cassette with this denomination already exists in the dispenser.");
        }

        var cassette = CashCassette.Create(denomination, capacity);
        _cassettes.Add(cassette);
        return Result.Success();
    }

    public bool CanDispense(IReadOnlyDictionary<Denomination, int> denominations)
    {
        if (denominations is null || denominations.Count == 0)
        {
            return false;
        }

        if (denominations.Values.Any(q => q <= 0))
        {
            return false;
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Denomination.Equals(denomination));

            if (cassette is null)
            {
                return false;
            }

            if (!cassette.CanReserve(quantity))
            {
                return false;
            }
        }

        return true;
    }

    public Result ReserveCash(IReadOnlyDictionary<Denomination, int> denominations)
    {
        if (denominations is null || denominations.Count == 0)
        {
            return Result.Failure("At least one denomination must be specified.");
        }

        if (denominations.Values.Any(q => q <= 0))
        {
            return Result.Failure("Quantity must be greater than zero.");
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Denomination.Equals(denomination));

            if (cassette is null)
            {
                return Result.Failure("The specified denomination was not found in the dispenser.");
            }

            if (!cassette.CanReserve(quantity))
            {
                return Result.Failure("Insufficient cash to complete this operation.");
            }
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(denomination));
            cassette.Reserve(quantity);
        }

        return Result.Success();
    }

    public void DispenseCash(IReadOnlyDictionary<Denomination, int> denominations)
    {
        foreach (var (denomination, quantity) in denominations)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(denomination));
            cassette.Dispense(quantity);
        }
    }

    public void ReleaseCash(IReadOnlyDictionary<Denomination, int> denominations)
    {
        foreach (var (denomination, quantity) in denominations)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(denomination));
            cassette.Release(quantity);
        }
    }

    public Result ReplenishCassette(CassetteId cassetteId, int count)
    {
        var cassette = _cassettes.FirstOrDefault(c => c.Id.Equals(cassetteId));

        if (cassette is null)
        {
            return Result.Failure("The specified cassette was not found in the dispenser.");
        }

        cassette.Replenish(count);
        return Result.Success();
    }

    public decimal GetAvailableCash()
    {
        return _cassettes.Sum(c => c.Denomination.Value * c.AvailableCount);
    }

    public bool HasCash()
    {
        return _cassettes.Any(c => c.CurrentCount > 0);
    }
}
