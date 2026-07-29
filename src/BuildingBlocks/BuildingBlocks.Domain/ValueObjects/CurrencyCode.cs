using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.ValueObjects;

public sealed class CurrencyCode : ValueObject
{
    public string Value { get; }

    private CurrencyCode(string value)
    {
        Value = value;
    }

    public static CurrencyCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Currency code cannot be empty.");
        }

        var normalized = code.ToUpperInvariant();

        if (normalized.Length != 3)
        {
            throw new DomainException("Currency code must be exactly 3 characters.");
        }

        if (!normalized.All(char.IsLetter))
        {
            throw new DomainException("Currency code must contain only alphabetic characters.");
        }

        return new CurrencyCode(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
