using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class DispensePlan : ValueObject
{
    private readonly IReadOnlyCollection<DispenseItem> _items;

    public IReadOnlyCollection<DispenseItem> Items => _items;
    public Money TotalAmount { get; }
    public bool IsEmpty => _items.Count == 0;

    private DispensePlan(IReadOnlyCollection<DispenseItem> items, Money totalAmount)
    {
        _items = items;
        TotalAmount = totalAmount;
    }

    public static DispensePlan Create(IEnumerable<DispenseItem> items, Currency currency)
    {
        var list = items.Where(i => i.BillCount > 0).ToList();

        if (list.Count == 0)
        {
            return Empty(currency);
        }

        var total = list
            .Select(i => i.TotalValue)
            .Aggregate(Money.Zero(currency), (current, value) => current.Add(value));

        return new DispensePlan(list.AsReadOnly(), total);
    }

    public static DispensePlan Empty(Currency currency)
    {
        return new DispensePlan(Array.Empty<DispenseItem>(), Money.Zero(currency));
    }

    public int TotalBills()
    {
        return _items.Sum(i => i.BillCount);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TotalAmount;

        foreach (var item in _items)
        {
            yield return item;
        }
    }
}
