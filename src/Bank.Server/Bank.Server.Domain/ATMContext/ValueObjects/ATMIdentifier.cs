using BuildingBlocks.Domain;

namespace Bank.Server.Domain.ATMContext.ValueObjects
{
    public sealed class ATMIdentifier(string Value) : ValueObject
    {
        public string Value { get; }
        public static ATMIdentifier Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Invalid account number");

            return new ATMIdentifier(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}
