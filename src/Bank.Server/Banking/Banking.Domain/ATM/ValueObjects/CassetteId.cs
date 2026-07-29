using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class CassetteId : ValueObject
{
    public Guid Value { get; }

    private CassetteId(Guid value)
    {
        Value = value;
    }

    public static CassetteId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("Cassette identifier cannot be empty.");
        }

        return new CassetteId(value);
    }

    public static CassetteId New()
    {
        return new CassetteId(Guid.NewGuid());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
