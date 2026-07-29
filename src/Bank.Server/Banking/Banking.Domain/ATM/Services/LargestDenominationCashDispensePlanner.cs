using Banking.Domain.ATM.Entities;
using Banking.Domain.ATM.ValueObjects;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.Services;

public sealed class LargestDenominationCashDispensePlanner : ICashDispensePlanner
{
    public Result<DispensePlan> CreatePlan(
        IReadOnlyCollection<CashCassette> cassettes,
        Money requestedAmount)
    {
        if (requestedAmount.IsZero() || requestedAmount.IsNegative())
        {
            return Result<DispensePlan>.Failure("Requested amount must be positive.");
        }

        var candidates = cassettes
            .Where(c => c.AvailableCount > 0)
            .OrderByDescending(c => c.Denomination.Value);

        var items = new List<DispenseItem>();
        var remaining = requestedAmount.Amount;

        foreach (var cassette in candidates)
        {
            if (remaining < cassette.Denomination.Value)
            {
                continue;
            }

            var maxNotes = (int)(remaining / cassette.Denomination.Value);
            var notesToUse = Math.Min(maxNotes, cassette.AvailableCount);

            if (notesToUse <= 0)
            {
                continue;
            }

            var denomMoney = Money.Create(cassette.Denomination.Value, requestedAmount.Currency);
            items.Add(DispenseItem.Create(cassette.Id, denomMoney, notesToUse));
            remaining -= notesToUse * cassette.Denomination.Value;

            if (remaining == 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            return Result<DispensePlan>.Failure("Insufficient cash to satisfy the requested amount.");
        }

        var plan = DispensePlan.Create(items, requestedAmount.Currency);
        return Result<DispensePlan>.Success(plan);
    }
}
