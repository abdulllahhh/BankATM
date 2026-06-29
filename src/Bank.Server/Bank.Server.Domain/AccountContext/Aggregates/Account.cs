using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AccountContext.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Bank.Server.Domain.AccountContext.Aggregates
{

    public sealed class Account
        : AggregateRoot<Guid>
    {
        private Account()
        {
            // EF Core
        }

        public AccountNumber AccountNumber { get; private set; }

        public Money Balance { get; private set; }

        public Money DailyLimit { get; private set; }

        public Money WithdrawnToday { get; private set; }

        public AccountStatus Status { get; private set; }

        public static Account Create(
            AccountNumber accountNumber,
            Money openingBalance,
            Money dailyLimit)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = accountNumber,
                Balance = openingBalance,
                DailyLimit = dailyLimit,
                WithdrawnToday =
                    Money.Create(0, openingBalance.Currency),
                Status = AccountStatus.Active
            };

            account.RaiseDomainEvent(
                new AccountCreatedDomainEvent(
                    account.Id));

            return account;
        }

        public Result Withdraw(Money amount)
        {
            // business rules here

            return Result.Success();
        }
    }
}