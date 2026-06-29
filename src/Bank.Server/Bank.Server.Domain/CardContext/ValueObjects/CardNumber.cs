using BuildingBlocks.Domain;

namespace Bank.Server.Domain.CardContext.ValueObjects
{
    public sealed class CardNumber(string Value) : ValueObject
    {
        public string Value { get; }
        public static CardNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Invalid account number");

            return new CardNumber(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
