using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class ATMLocation : ValueObject
{
    public string Value { get; }

    private ATMLocation(string value)
    {
        Value = value;
    }

    public static ATMLocation Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("ATM location cannot be empty.");
        }

        return new ATMLocation(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
