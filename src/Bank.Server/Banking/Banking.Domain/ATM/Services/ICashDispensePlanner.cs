using Banking.Domain.ATM.Entities;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.Services;

public interface ICashDispensePlanner
{
    Result<DispensePlan> CreatePlan(
        IReadOnlyCollection<CashCassette> cassettes,
        Money requestedAmount);
}
