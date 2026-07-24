using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.TransactionContext.DomainEvents;
using Bank.Server.Domain.TransactionContext.ValueObjects;
using BuildingBlocks.Domain.Common;


namespace Bank.Server.Domain.TransactionContext.Aggregates
{
    public sealed class Transaction
        : AggregateRoot<Guid>
    {
        private Transaction()
        {
        }

        public TransactionStatus Status { get; private set; }

        public TransactionType Type { get; private set; }

        public Money Amount { get; private set; }

        public Guid? FromAccountId { get; private set; }

        public Guid? ToAccountId { get; private set; }

        public static Transaction CreateWithdrawal(
            Guid transactionId,
            Guid accountId,
            Money amount)
        {
            if (transactionId == Guid.Empty)
                throw new ArgumentException("Transaction id must not be empty.", nameof(transactionId));

            if (accountId == Guid.Empty)
                throw new ArgumentException("Account id must not be empty.", nameof(accountId));

            return new Transaction
            {
                Id = transactionId,
                Type = TransactionType.Withdraw,
                Status = TransactionStatus.Completed,
                Amount = amount,
                FromAccountId = accountId
            };
        }

        public void Approve()
        {
            EnsurePending();

            Status = TransactionStatus.Approved;

            RaiseDomainEvent(
                new TransactionApprovedDomainEvent(Id));
        }

        public void Complete()
        {
            EnsureApproved();

            Status = TransactionStatus.Completed;

            RaiseDomainEvent(
                new TransactionCompletedDomainEvent(Id));
        }

        public void Cancel()
        {
            EnsurePending();

            Status = TransactionStatus.Cancelled;

            RaiseDomainEvent(
                new TransactionCancelledDomainEvent(Id));
        }

        private void EnsurePending()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException();
        }

        private void EnsureApproved()
        {
            if (Status != TransactionStatus.Approved)
                throw new InvalidOperationException();
        }
    }
}
