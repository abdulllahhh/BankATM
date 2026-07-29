using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.ValueObjects;

public sealed class Currency : ValueObject
{
    public string Code { get; }
    public string Symbol { get; }

    private Currency(string code, string symbol)
    {
        Code = code;
        Symbol = symbol;
    }

    public static Currency Create(string code, string symbol)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Currency code cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new DomainException("Currency symbol cannot be empty.");
        }

        return new Currency(code.ToUpperInvariant(), symbol);
    }

    public static readonly Currency USD = new("USD", "$");
    public static readonly Currency EUR = new("EUR", "\u20AC");
    public static readonly Currency EGP = new("EGP", "\u00A3");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }
}
