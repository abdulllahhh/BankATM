using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class Denomination : ValueObject
{
    public decimal Value { get; }

    private Denomination(decimal value)
    {
        Value = value;
    }

    public static Denomination Create(decimal value)
    {
        if (value <= 0)
        {
            throw new DomainException("Denomination must be greater than zero.");
        }

        return new Denomination(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
