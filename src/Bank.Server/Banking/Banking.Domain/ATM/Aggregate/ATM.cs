using Banking.Domain.ATM.Entities;
using Banking.Domain.ATM.Enums;
using Banking.Domain.ATM.Errors;
using Banking.Domain.ATM.Events;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.Aggregate;

public sealed class ATM : AggregateRoot<ATMId>
{
    private readonly List<CashInventory> _cashInventory = new();

    public ATMLocation Location { get; private set; } = null!;
    public ATMStatus Status { get; private set; }
    public IReadOnlyCollection<CashInventory> Inventory => _cashInventory.AsReadOnly();
    public DateTime StartedAt { get; private set; }
    public DateTime? LastMaintenance { get; private set; }

    private ATM() { }

    private ATM(ATMId id, ATMLocation location) : base(id)
    {
        Location = location;
        Status = ATMStatus.Online;
        StartedAt = DateTime.UtcNow;
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

    public Result ReserveCash(IReadOnlyDictionary<decimal, int> denominations)
    {
        if (Status != ATMStatus.Online)
        {
            return Result.Failure(ATMErrors.NotOnline);
        }

        if (denominations is null || denominations.Count == 0)
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        if (denominations.Values.Any(q => q <= 0))
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var slot = _cashInventory.FirstOrDefault(c => c.Denomination == denomination);

            if (slot is null)
            {
                return Result.Failure(ATMErrors.DenominationNotFound);
            }

            if (!slot.CanReserve(quantity))
            {
                return Result.Failure(ATMErrors.InsufficientCash);
            }
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var slot = _cashInventory.First(c => c.Denomination == denomination);
            slot.Reserve(quantity);
        }

        var totalAmount = denominations.Sum(d => d.Key * d.Value);
        RaiseDomainEvent(new CashReservedDomainEvent(Id, totalAmount));
        return Result.Success();
    }

    public Result DispenseCash(IReadOnlyDictionary<decimal, int> denominations)
    {
        if (Status != ATMStatus.Online)
        {
            return Result.Failure(ATMErrors.NotOnline);
        }

        if (denominations is null || denominations.Count == 0)
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        if (denominations.Values.Any(q => q <= 0))
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var slot = _cashInventory.FirstOrDefault(c => c.Denomination == denomination);

            if (slot is null)
            {
                return Result.Failure(ATMErrors.DenominationNotFound);
            }

            if (slot.ReservedCount < quantity)
            {
                return Result.Failure(ATMErrors.CashNotReserved);
            }
        }

        foreach (var (denomination, quantity) in denominations)
        {
            var slot = _cashInventory.First(c => c.Denomination == denomination);
            slot.Dispense(quantity);
        }

        var totalAmount = denominations.Sum(d => d.Key * d.Value);
        RaiseDomainEvent(new CashDispensedDomainEvent(Id, totalAmount));

        if (_cashInventory.All(c => c.Count == 0))
        {
            Status = ATMStatus.OutOfCash;
            RaiseDomainEvent(new ATMOutOfCashDomainEvent(Id));
        }

        return Result.Success();
    }

    public Result ReplenishCash(decimal denomination, int count)
    {
        if (Status is not (ATMStatus.Online or ATMStatus.Maintenance or ATMStatus.OutOfCash))
        {
            return Result.Failure(ATMErrors.CannotReplenish);
        }

        if (denomination <= 0)
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        if (count <= 0)
        {
            return Result.Failure(ATMErrors.InvalidQuantity);
        }

        var slot = _cashInventory.FirstOrDefault(c => c.Denomination == denomination);

        if (slot is null)
        {
            slot = CashInventory.Create(denomination, 0);
            _cashInventory.Add(slot);
        }

        slot.Replenish(count);

        if (Status == ATMStatus.OutOfCash)
        {
            Status = ATMStatus.Online;
        }

        RaiseDomainEvent(new CashReplenishedDomainEvent(Id, denomination, count));
        return Result.Success();
    }
}
