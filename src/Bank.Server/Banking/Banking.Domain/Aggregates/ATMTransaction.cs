using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Aggregates;

public sealed class ATMTransaction : AggregateRoot<Guid>
{
    public Guid ATMId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid DebitCardId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? FailureReason { get; private set; }

    private ATMTransaction() { }

    public ATMTransaction(
        Guid id,
        Guid atmId,
        Guid accountId,
        Guid debitCardId,
        decimal amount,
        string currency,
        TransactionType type)
        : base(id)
    {
        ATMId = atmId;
        AccountId = accountId;
        DebitCardId = debitCardId;
        Amount = amount;
        Currency = currency;
        Type = type;
        Status = TransactionStatus.Pending;
        Timestamp = DateTime.UtcNow;
    }
}

public enum TransactionType
{
    Withdrawal,
    Deposit,
    BalanceInquiry
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}
