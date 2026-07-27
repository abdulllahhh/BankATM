using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class CardNumber : ValueObject
{
    public string Value { get; }

    private CardNumber(string value)
    {
        Value = value;
    }

    public static CardNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Card number cannot be empty.");
        }

        var sanitized = new string(value.Where(char.IsDigit).ToArray());

        if (sanitized.Length < 13 || sanitized.Length > 19)
        {
            throw new DomainException("Card number must be between 13 and 19 digits.");
        }

        if (!PassesLuhn(sanitized))
        {
            throw new DomainException("Card number failed Luhn validation.");
        }

        return new CardNumber(sanitized);
    }

    private static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var alternate = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var digit = digits[i] - '0';
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }
            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
