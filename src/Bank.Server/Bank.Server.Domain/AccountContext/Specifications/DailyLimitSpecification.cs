using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.BaseSpecification;


namespace Bank.Server.Domain.AccountContext.Specifications
{
    public sealed class DailyLimitSpecification
        : Specification<Account>
    {
        private readonly Money _amount;

        public DailyLimitSpecification(
            Money amount)
        {
            _amount = amount;
        }

        public override bool IsSatisfiedBy(
            Account account)
        {
            return account.WithdrawnToday.Amount +
                   _amount.Amount
                   <=
                   account.DailyLimit.Amount;
        }
    }
}
