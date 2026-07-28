using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class ATMId : ValueObject
{
    public Guid Value { get; }

    private ATMId(Guid value)
    {
        Value = value;
    }

    public static ATMId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("ATM identifier cannot be empty.");
        }

        return new ATMId(value);
    }

    public static ATMId New()
    {
        return new ATMId(Guid.NewGuid());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
