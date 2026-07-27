using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class IssueDate : ValueObject
{
    public DateOnly Value { get; }

    private IssueDate(DateOnly value)
    {
        Value = value;
    }

    public static IssueDate Create(DateOnly value)
    {
        if (value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException("Issue date cannot be in the future.");
        }

        return new IssueDate(value);
    }

    public static IssueDate Today()
    {
        return new IssueDate(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
