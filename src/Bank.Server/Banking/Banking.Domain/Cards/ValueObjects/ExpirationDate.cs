using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class ExpirationDate : ValueObject
{
    public DateOnly Value { get; }
    public bool IsExpired => Value < DateOnly.FromDateTime(DateTime.UtcNow);

    private ExpirationDate(DateOnly value)
    {
        Value = value;
    }

    public static ExpirationDate From(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (value < today)
            throw new ArgumentException("Expiration date must be in the future.", nameof(value));

        return new ExpirationDate(value);
    }

    public static ExpirationDate From(int month, int year)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");

        if (year < 100)
            year += 2000;

        return From(new DateOnly(year, month, 1).AddMonths(1).AddDays(-1));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("MM/yy");
}
