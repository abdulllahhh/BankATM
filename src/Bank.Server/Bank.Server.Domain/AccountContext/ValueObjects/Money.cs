using BuildingBlocks.Domain.Common;


namespace Bank.Server.Domain.AccountContext.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }

        public string Currency { get; }

        private Money() { }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException();

            return new Money(amount, currency);
        }

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);

            return new(
                Amount + other.Amount,
                Currency);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);

            return new(
                Amount - other.Amount,
                Currency);
        }

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
