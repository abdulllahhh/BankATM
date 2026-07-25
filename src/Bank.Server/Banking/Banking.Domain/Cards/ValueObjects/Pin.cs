using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class Pin : ValueObject
{
    public string Value { get; }

    private Pin(string value)
    {
        Value = value;
    }

    public static Pin From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PIN cannot be empty.", nameof(value));

        if (value.Length is < 4 or > 6)
            throw new ArgumentException("PIN must be between 4 and 6 digits.", nameof(value));

        if (!value.All(char.IsDigit))
            throw new ArgumentException("PIN must contain only digits.", nameof(value));

        return new Pin(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => new string('*', Value.Length);
}
