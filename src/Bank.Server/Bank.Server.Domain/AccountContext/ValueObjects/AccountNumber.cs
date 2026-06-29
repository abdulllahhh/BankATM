using BuildingBlocks.Domain;


namespace Bank.Server.Domain.AccountContext.ValueObjects
{
    public sealed class AccountNumber : ValueObject
    {
        public string Value { get; }

        private AccountNumber(string value)
        {
            Value = value;
        }
        private AccountNumber() { }

        public static AccountNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Invalid account number");

            return new AccountNumber(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
