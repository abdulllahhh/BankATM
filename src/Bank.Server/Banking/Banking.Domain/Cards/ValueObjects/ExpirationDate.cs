using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class ExpirationDate : ValueObject
{
    public DateOnly Value { get; }

    private ExpirationDate(DateOnly value)
    {
        Value = value;
    }

    public static ExpirationDate Create(DateOnly value, IssueDate issueDate)
    {
        if (value <= issueDate.Value)
        {
            throw new DomainException("Expiration date must be after the issue date.");
        }

        return new ExpirationDate(value);
    }

    public bool IsExpired()
    {
        return Value < DateOnly.FromDateTime(DateTime.UtcNow);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
