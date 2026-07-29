using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class DispensePlan : ValueObject
{
    private readonly IReadOnlyCollection<DispenseItem> _items;

    public IReadOnlyCollection<DispenseItem> Items => _items;
    public decimal TotalAmount => _items.Sum(i => i.Denomination.Value * i.Quantity);
    public bool IsEmpty => _items.Count == 0;

    private DispensePlan(IReadOnlyCollection<DispenseItem> items)
    {
        _items = items;
    }

    public static DispensePlan Create(IEnumerable<DispenseItem> items)
    {
        var list = items.Where(i => i.Quantity > 0).ToList();

        if (list.Count == 0)
        {
            return Empty();
        }

        return new DispensePlan(list.AsReadOnly());
    }

    public static DispensePlan Empty()
    {
        return new DispensePlan(Array.Empty<DispenseItem>());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        foreach (var item in _items)
        {
            yield return item;
        }
    }
}
