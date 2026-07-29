using Banking.Domain.ATM.DomainServices;
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

    public DispensePlan CreatePlan(decimal amount, ICashDispenseStrategy strategy)
    {
        var cassetteInfos = _cassettes
            .Select(c => new CassetteInfo(c.Denomination, c.AvailableCount))
            .ToList();

        return strategy.CreatePlan(amount, cassetteInfos);
    }

    public Result ReserveCash(DispensePlan plan)
    {
        if (plan.IsEmpty)
        {
            return Result.Failure("Cannot reserve an empty plan.");
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Denomination.Equals(item.Denomination));

            if (cassette is null)
            {
                return Result.Failure("Plan references a denomination not present in the dispenser.");
            }

            if (!cassette.CanReserve(item.Quantity))
            {
                return Result.Failure("Insufficient available cash to complete this reservation.");
            }
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(item.Denomination));
            cassette.Reserve(item.Quantity);
        }

        return Result.Success();
    }

    public Result DispenseCash(DispensePlan plan)
    {
        if (plan.IsEmpty)
        {
            return Result.Failure("Cannot dispense an empty plan.");
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Denomination.Equals(item.Denomination));

            if (cassette is null)
            {
                return Result.Failure("Plan references a denomination not present in the dispenser.");
            }

            if (cassette.ReservedCount < item.Quantity)
            {
                return Result.Failure("Cannot dispense more than the reserved quantity.");
            }
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(item.Denomination));
            cassette.Dispense(item.Quantity);
        }

        return Result.Success();
    }

    public void ReleaseCash(DispensePlan plan)
    {
        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.First(c => c.Denomination.Equals(item.Denomination));
            cassette.Release(item.Quantity);
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
