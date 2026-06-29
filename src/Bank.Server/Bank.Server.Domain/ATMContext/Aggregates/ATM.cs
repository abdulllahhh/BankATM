using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.ATMContext.DomainEvents;
using Bank.Server.Domain.ATMContext.ValueObjects;
using BuildingBlocks.Domain;

namespace Bank.Server.Domain.ATMContext.Aggregates
{
    public sealed class ATM : AggregateRoot<Guid>
    {
        public Money CashAvailable { get; private set; }
        public ATMIdentifier ATMIdentifier { get; private set; }

        public ATMStatus Status { get; private set; }

        public Result DispenseCash(Money amount)
        {
            if (Status != ATMStatus.Online)
                return Result.Failure(
                    "ATM offline");

            if (CashAvailable.Amount <
                amount.Amount)
            {
                return Result.Failure(
                    "Insufficient ATM cash");
            }

            CashAvailable =
                CashAvailable.Subtract(amount);

            RaiseDomainEvent(
                new CashDispensedDomainEvent(
                    Id,
                    amount.Amount));

            return Result.Success();
        }

        public void LoadCash(Money amount)
        {
            CashAvailable =
                CashAvailable.Add(amount);

            RaiseDomainEvent(
                new CashLoadedDomainEvent(
                    Id,
                    amount.Amount));
        }
    }
}
