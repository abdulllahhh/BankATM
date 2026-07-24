using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Aggregates;

public sealed class DebitCard : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public string CardNumber { get; private set; } = null!;
    public DateOnly ExpiryDate { get; private set; }
    public DebitCardStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DebitCard() { }

    public DebitCard(Guid id, Guid accountId, string cardNumber, DateOnly expiryDate)
        : base(id)
    {
        AccountId = accountId;
        CardNumber = cardNumber;
        ExpiryDate = expiryDate;
        Status = DebitCardStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}

public enum DebitCardStatus
{
    Active,
    Blocked,
    Expired
}
