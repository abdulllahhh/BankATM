using Banking.Domain.ATM.ValueObjects;

namespace Banking.Domain.ATM.DomainServices;

public sealed class LargestDenominationFirstStrategy : ICashDispenseStrategy
{
    public DispensePlan CreatePlan(decimal amount, IReadOnlyCollection<CassetteInfo> availableCassettes)
    {
        if (amount <= 0)
        {
            return DispensePlan.Empty();
        }

        var candidates = availableCassettes
            .Where(c => c.AvailableCount > 0)
            .OrderByDescending(c => c.Denomination.Value);

        var items = new List<DispenseItem>();
        var remaining = amount;

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

            items.Add(new DispenseItem(cassette.Denomination, notesToUse));
            remaining -= notesToUse * cassette.Denomination.Value;

            if (remaining == 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            return DispensePlan.Empty();
        }

        return DispensePlan.Create(items);
    }
}
