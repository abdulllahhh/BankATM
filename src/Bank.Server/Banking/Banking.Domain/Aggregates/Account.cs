using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Aggregates;

public sealed class Account : AggregateRoot<Guid>
{
    public string AccountHolder { get; private set; } = null!;
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = null!;
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Account() { }

    public Account(Guid id, string accountHolder, decimal balance, string currency)
        : base(id)
    {
        AccountHolder = accountHolder;
        Balance = balance;
        Currency = currency;
        Status = AccountStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}

public enum AccountStatus
{
    Active,
    Frozen,
    Closed
}
