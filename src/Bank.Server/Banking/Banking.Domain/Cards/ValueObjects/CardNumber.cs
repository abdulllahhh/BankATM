using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class CardNumber : ValueObject
{
    public string Value { get; }
    public string LastFourDigits { get; }

    private CardNumber(string value)
    {
        Value = value;
        LastFourDigits = value.Length >= 4
            ? value[^4..]
            : value;
    }

    public static CardNumber From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Card number cannot be empty.", nameof(value));

        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (digits.Length != 16)
            throw new ArgumentException("Card number must be exactly 16 digits.", nameof(value));

        if (!IsValidLuhn(digits))
            throw new ArgumentException("Card number failed Luhn validation.", nameof(value));

        return new CardNumber(digits);
    }

    private static bool IsValidLuhn(string digits)
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
                    digit -= 9;
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

    public override string ToString() => $"**** **** **** {LastFourDigits}";
}
