using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class CardId : ValueObject
{
    public Guid Value { get; }

    private CardId(Guid value)
    {
        Value = value;
    }

    public static CardId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("Card identifier cannot be empty.");
        }

        return new CardId(value);
    }

    public static CardId New()
    {
        return new CardId(Guid.NewGuid());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
