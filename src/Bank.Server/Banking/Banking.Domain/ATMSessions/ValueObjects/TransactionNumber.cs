using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATMSessions.ValueObjects;

public sealed class TransactionNumber : ValueObject
{
    public string Value { get; }

    private TransactionNumber(string value)
    {
        Value = value;
    }

    public static TransactionNumber Generate()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Random.Shared.Next(1000, 9999);
        return new TransactionNumber($"TXN-{timestamp}-{random}");
    }

    public static TransactionNumber From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Transaction number cannot be empty", nameof(value));

        return new TransactionNumber(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
