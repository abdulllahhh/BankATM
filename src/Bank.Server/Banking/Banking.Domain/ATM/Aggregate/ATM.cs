using Banking.Domain.ATM.Entities;
using Banking.Domain.ATM.Enums;
using Banking.Domain.ATM.Errors;
using Banking.Domain.ATM.Events;
using Banking.Domain.ATM.Services;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.Aggregate;

public sealed class ATM : AggregateRoot<ATMId>
{
    private CashDispenser _dispenser = null!;

    public ATMLocation Location { get; private set; } = null!;
    public ATMStatus Status { get; private set; }
    public IReadOnlyCollection<CashCassette> Cassettes => _dispenser.Cassettes;
    public DateTime StartedAt { get; private set; }
    public DateTime? LastMaintenance { get; private set; }

    private ATM() { }

    private ATM(ATMId id, ATMLocation location) : base(id)
    {
        Location = location;
        Status = ATMStatus.Online;
        StartedAt = DateTime.UtcNow;
        _dispenser = CashDispenser.Create();
    }

    public static ATM Create(ATMId id, ATMLocation location)
    {
        var atm = new ATM(id, location);
        atm.RaiseDomainEvent(new ATMStartedDomainEvent(id));
        return atm;
    }

    public Result Start()
    {
        if (Status != ATMStatus.Offline)
        {
            return Result.Failure(ATMErrors.CannotStart);
        }

        Status = ATMStatus.Online;
        RaiseDomainEvent(new ATMStartedDomainEvent(Id));
        return Result.Success();
    }

    public Result Shutdown()
    {
        if (Status is not (ATMStatus.Online or ATMStatus.Maintenance))
        {
            return Result.Failure(ATMErrors.CannotShutdown);
        }

        Status = ATMStatus.Offline;
        RaiseDomainEvent(new ATMShutdownDomainEvent(Id));
        return Result.Success();
    }

    public Result StartMaintenance()
    {
        if (Status != ATMStatus.Online)
        {
            return Result.Failure(ATMErrors.CannotStartMaintenance);
        }

        Status = ATMStatus.Maintenance;
        LastMaintenance = DateTime.UtcNow;
        RaiseDomainEvent(new MaintenanceStartedDomainEvent(Id));
        return Result.Success();
    }

    public Result CompleteMaintenance()
    {
        if (Status != ATMStatus.Maintenance)
        {
            return Result.Failure(ATMErrors.CannotCompleteMaintenance);
        }

        Status = ATMStatus.Online;
        RaiseDomainEvent(new MaintenanceCompletedDomainEvent(Id));
        return Result.Success();
    }

    public Result ConfigureCassette(Denomination denomination, int capacity)
    {
        if (Status != ATMStatus.Maintenance)
        {
            return Result.Failure(ATMErrors.CannotConfigureCassette);
        }

        return _dispenser.AddCassette(denomination, capacity);
    }

    public bool CanDispense(Money amount, ICashDispensePlanner planner)
    {
        return Status == ATMStatus.Online && _dispenser.CanDispense(amount, planner);
    }

    public Money GetAvailableCash(Currency currency)
    {
        return _dispenser.GetAvailableCash(currency);
    }

    public Result<DispensePlan> ReserveCash(Money amount, ICashDispensePlanner planner)
    {
        if (Status != ATMStatus.Online)
        {
            return Result<DispensePlan>.Failure(ATMErrors.NotOnline);
        }

        var result = _dispenser.ReserveCash(amount, planner);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(new CashReservedDomainEvent(Id, result.Value!.TotalAmount));
        return result;
    }

    public Result ExecutePlan(DispensePlan plan)
    {
        if (Status != ATMStatus.Online)
        {
            return Result.Failure(ATMErrors.NotOnline);
        }

        var result = _dispenser.ExecutePlan(plan);

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(new CashDispensedDomainEvent(Id, plan.TotalAmount));

        if (!_dispenser.HasCash())
        {
            Status = ATMStatus.OutOfCash;
            RaiseDomainEvent(new ATMOutOfCashDomainEvent(Id));
        }

        return Result.Success();
    }

    public Result ReplenishCassette(CassetteId cassetteId, int count)
    {
        if (Status is not (ATMStatus.Online or ATMStatus.Maintenance or ATMStatus.OutOfCash))
        {
            return Result.Failure(ATMErrors.CannotReplenish);
        }

        if (count <= 0)
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        var cassette = _dispenser.Cassettes.FirstOrDefault(c => c.Id.Equals(cassetteId));

        if (cassette is null)
        {
            return Result.Failure(ATMErrors.CassetteNotFound);
        }

        var result = _dispenser.ReplenishCassette(cassetteId, count);

        if (result.IsFailure)
        {
            return result;
        }

        if (Status == ATMStatus.OutOfCash)
        {
            Status = ATMStatus.Online;
        }

        RaiseDomainEvent(new CashReplenishedDomainEvent(Id, cassette.Denomination, cassetteId, count));
        return Result.Success();
    }
}
