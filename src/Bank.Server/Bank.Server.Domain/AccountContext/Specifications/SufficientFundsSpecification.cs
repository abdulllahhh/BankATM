using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.BaseSpecification;


namespace Bank.Server.Domain.AccountContext.Specifications
{
    public sealed class SufficientFundsSpecification
        : Specification<Account>
    {
        private readonly Money _amount;

        public SufficientFundsSpecification(
            Money amount)
        {
            _amount = amount;
        }

        public override bool IsSatisfiedBy(
            Account account)
        {
            return account.Balance.Amount >=
                   _amount.Amount;
        }
    }
}
