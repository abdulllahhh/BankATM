using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.ValueObjects;

public sealed class Currency : ValueObject
{
    public CurrencyCode Code { get; }
    public string Name { get; }
    public string Symbol { get; }
    public int NumericCode { get; }
    public byte MinorUnit { get; }

    private Currency(CurrencyCode code, string name, string symbol, int numericCode, byte minorUnit)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        NumericCode = numericCode;
        MinorUnit = minorUnit;
    }

    public static Currency Create(CurrencyCode code, string name, string symbol, int numericCode, byte minorUnit)
    {
        if (code is null)
        {
            throw new DomainException("Currency code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Currency name is required.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new DomainException("Currency symbol is required.");
        }

        return new Currency(code, name, symbol, numericCode, minorUnit);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => $"{Code} ({Name})";
}
