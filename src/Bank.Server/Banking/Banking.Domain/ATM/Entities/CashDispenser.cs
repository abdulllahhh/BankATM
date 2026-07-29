using Banking.Domain.ATM.Services;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

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

    public bool CanDispense(Money amount, ICashDispensePlanner planner)
    {
        if (amount.IsZero() || amount.IsNegative())
        {
            return false;
        }

        var planResult = planner.CreatePlan(_cassettes, amount);
        return planResult.IsSuccess && !planResult.Value!.IsEmpty;
    }

    public Result<DispensePlan> ReserveCash(Money amount, ICashDispensePlanner planner)
    {
        if (amount.IsZero() || amount.IsNegative())
        {
            return Result<DispensePlan>.Failure("Amount must be positive.");
        }

        var planResult = planner.CreatePlan(_cassettes, amount);

        if (planResult.IsFailure)
        {
            return planResult;
        }

        var plan = planResult.Value!;

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Id.Equals(item.CassetteId));

            if (cassette is null)
            {
                return Result<DispensePlan>.Failure("Plan references a cassette not present in the dispenser.");
            }

            if (!cassette.CanReserve(item.BillCount))
            {
                return Result<DispensePlan>.Failure("Insufficient available cash to complete this reservation.");
            }
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.First(c => c.Id.Equals(item.CassetteId));
            cassette.Reserve(item.BillCount);
        }

        return Result<DispensePlan>.Success(plan);
    }

    public Result ExecutePlan(DispensePlan plan)
    {
        if (plan.IsEmpty)
        {
            return Result.Failure("Cannot execute an empty plan.");
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.FirstOrDefault(c => c.Id.Equals(item.CassetteId));

            if (cassette is null)
            {
                return Result.Failure("Plan references a cassette not present in the dispenser.");
            }

            if (cassette.ReservedCount < item.BillCount)
            {
                return Result.Failure("Cannot dispense more than the reserved quantity.");
            }
        }

        foreach (var item in plan.Items)
        {
            var cassette = _cassettes.First(c => c.Id.Equals(item.CassetteId));
            cassette.Dispense(item.BillCount);
        }

        return Result.Success();
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

    public Money GetAvailableCash(Currency currency)
    {
        var total = _cassettes.Sum(c => c.Denomination.Value * c.AvailableCount);
        return Money.Create(total, currency);
    }

    public bool HasCash()
    {
        return _cassettes.Any(c => c.CurrentCount > 0);
    }
}
