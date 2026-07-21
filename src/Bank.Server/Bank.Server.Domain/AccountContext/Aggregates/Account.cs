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

        public Result Withdraw(Money amount, Guid atmId, Guid transactionId = default)
        {
            if (atmId == Guid.Empty)
            {
                return Result.Failure("ATM id is required.");
            }

            if (transactionId == Guid.Empty)
            {
                transactionId = Guid.NewGuid();
            }

            if (Status != AccountStatus.Active)
            {
                return Result.Failure("Account is not active.");
            }

            if (Balance.Currency != amount.Currency)
            {
                return Result.Failure("Currency mismatch.");
            }

            if (WithdrawnToday.Amount + amount.Amount > DailyLimit.Amount)
            {
                RaiseDomainEvent(new DailyLimitExceededDomainEvent());
                return Result.Failure("Daily withdrawal limit exceeded.");
            }

            if (Balance.Amount < amount.Amount)
            {
                return Result.Failure("Insufficient funds.");
            }

            Balance = Balance.Subtract(amount);
            WithdrawnToday = WithdrawnToday.Add(amount);

            RaiseDomainEvent(new FundsWithdrawnDomainEvent(
                Id,
                atmId,
                amount.Amount,
                amount.Currency,
                transactionId));

            return Result.Success();
        }
    }
}
