using Banking.Domain.Cards.Enums;
using Banking.Domain.Cards.Errors;
using Banking.Domain.Cards.Events;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.Aggregate;

public sealed class DebitCard : AggregateRoot<Guid>
{
    private const int MaxFailedPinAttempts = 3;

    public Guid AccountId { get; private set; }
    public CardNumber CardNumber { get; private set; } = null!;
    public Pin Pin { get; private set; } = null!;
    public ExpirationDate ExpirationDate { get; private set; } = null!;
    public IssueDate IssueDate { get; private set; } = null!;
    public CardStatus Status { get; private set; }
    public int FailedAttempts { get; private set; }

    private DebitCard() { }

    private DebitCard(
        Guid id,
        Guid accountId,
        CardNumber cardNumber,
        Pin pin,
        ExpirationDate expirationDate,
        IssueDate issueDate)
        : base(id)
    {
        AccountId = accountId;
        CardNumber = cardNumber;
        Pin = pin;
        ExpirationDate = expirationDate;
        IssueDate = issueDate;
        Status = CardStatus.Active;
    }

    public static DebitCard Issue(
        Guid id,
        Guid accountId,
        CardNumber cardNumber,
        Pin pin,
        ExpirationDate expirationDate)
    {
        return new DebitCard(id, accountId, cardNumber, pin, expirationDate, IssueDate.Now());
    }

    public void Validate()
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.Validate.CardNotActive));
        Guard.CheckRule(new CardMustNotBeExpiredRule(ExpirationDate, CardErrors.Validate.CardExpired));

        RaiseDomainEvent(new CardValidatedDomainEvent(CardNumber, DateTime.UtcNow));
    }

    public void AuthenticatePin(Pin pin)
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.AuthenticatePin.CardNotActive));

        if (Pin.Equals(pin))
        {
            FailedAttempts = 0;
            RaiseDomainEvent(new PinAuthenticatedDomainEvent(CardNumber, DateTime.UtcNow));
        }
        else
        {
            FailedAttempts++;
            RaiseDomainEvent(new PinAuthenticationFailedDomainEvent(CardNumber, FailedAttempts, DateTime.UtcNow));

            if (FailedAttempts >= MaxFailedPinAttempts)
            {
                Status = CardStatus.Confiscated;
                RaiseDomainEvent(new CardConfiscatedDomainEvent(
                    CardNumber,
                    CardErrors.AuthenticatePin.CardNowConfiscated,
                    DateTime.UtcNow));
            }
        }
    }

    public void IncrementFailedAttempts()
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.IncrementFailedAttempts.CardNotActive));

        FailedAttempts++;

        if (FailedAttempts >= MaxFailedPinAttempts)
        {
            Status = CardStatus.Confiscated;
            RaiseDomainEvent(new CardConfiscatedDomainEvent(
                CardNumber,
                CardErrors.IncrementFailedAttempts.MaxAttemptsReached,
                DateTime.UtcNow));
        }
    }

    public void ResetFailedAttempts()
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.ResetFailedAttempts.CardNotActive));

        FailedAttempts = 0;
    }

    public void Confiscate(string reason)
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.Confiscate.CardNotActive));

        Status = CardStatus.Confiscated;
        RaiseDomainEvent(new CardConfiscatedDomainEvent(CardNumber, reason, DateTime.UtcNow));
    }

    public void Block(string reason)
    {
        Guard.CheckRule(new CardMustBeActiveRule(Status, CardErrors.Block.CardNotActive));

        Status = CardStatus.Blocked;
        RaiseDomainEvent(new CardBlockedDomainEvent(CardNumber, reason, DateTime.UtcNow));
    }

    public void Expire()
    {
        Guard.CheckRule(new CardMustNotBeTerminalRule(Status, CardErrors.Expire.AlreadyTerminal));

        Status = CardStatus.Expired;
    }

    private sealed class CardMustBeActiveRule : IBusinessRule
    {
        private readonly CardStatus _status;
        private readonly string _message;

        public CardMustBeActiveRule(CardStatus status, string message)
        {
            _status = status;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() => _status != CardStatus.Active;
    }

    private sealed class CardMustNotBeExpiredRule : IBusinessRule
    {
        private readonly ExpirationDate _expirationDate;
        private readonly string _message;

        public CardMustNotBeExpiredRule(ExpirationDate expirationDate, string message)
        {
            _expirationDate = expirationDate;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() => _expirationDate.IsExpired;
    }

    private sealed class CardMustNotBeTerminalRule : IBusinessRule
    {
        private readonly CardStatus _status;
        private readonly string _message;

        public CardMustNotBeTerminalRule(CardStatus status, string message)
        {
            _status = status;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() =>
            _status is CardStatus.Blocked or CardStatus.Expired or CardStatus.Confiscated;
    }
}
