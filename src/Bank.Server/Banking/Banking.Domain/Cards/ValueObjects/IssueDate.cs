using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class IssueDate : ValueObject
{
    public DateTime Value { get; }

    private IssueDate(DateTime value)
    {
        Value = value;
    }

    public static IssueDate From(DateTime value)
    {
        if (value > DateTime.UtcNow)
            throw new ArgumentException("Issue date cannot be in the future.", nameof(value));

        return new IssueDate(value);
    }

    public static IssueDate Now() => new(DateTime.UtcNow);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
