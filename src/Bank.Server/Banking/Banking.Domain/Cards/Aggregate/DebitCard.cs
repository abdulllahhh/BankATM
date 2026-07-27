using Banking.Domain.Cards.Enums;
using Banking.Domain.Cards.Errors;
using Banking.Domain.Cards.Events;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.Aggregate;

public sealed class DebitCard : AggregateRoot<Guid>
{
    public const int MaxFailedPinAttempts = 3;

    public CardNumber CardNumber { get; private set; } = null!;
    public Pin Pin { get; private set; } = null!;
    public IssueDate IssueDate { get; private set; } = null!;
    public ExpirationDate ExpirationDate { get; private set; } = null!;
    public CardStatus Status { get; private set; }
    public int FailedPinAttempts { get; private set; }

    private DebitCard()
    {
    }

    public static DebitCard Issue(
        CardNumber cardNumber,
        Pin pin,
        IssueDate issueDate,
        ExpirationDate expirationDate)
    {
        var card = new DebitCard
        {
            Id = Guid.NewGuid(),
            CardNumber = cardNumber,
            Pin = pin,
            IssueDate = issueDate,
            ExpirationDate = expirationDate,
            Status = CardStatus.Active,
            FailedPinAttempts = 0
        };

        return card;
    }

    public Result Validate()
    {
        if (ExpirationDate.IsExpired())
        {
            Status = CardStatus.Expired;
            return Result.Failure(CardErrors.CardExpired);
        }

        if (Status != CardStatus.Active)
        {
            return Result.Failure(CardErrors.CardNotActive);
        }

        RaiseDomainEvent(new CardValidatedDomainEvent(Id));

        return Result.Success();
    }

    public Result AuthenticatePin(Pin pin)
    {
        if (ExpirationDate.IsExpired())
        {
            Status = CardStatus.Expired;
            return Result.Failure(CardErrors.CardExpired);
        }

        if (Status == CardStatus.Blocked)
        {
            return Result.Failure(CardErrors.CardBlocked);
        }

        if (Status == CardStatus.Confiscated)
        {
            return Result.Failure(CardErrors.CardConfiscated);
        }

        if (Status != CardStatus.Active)
        {
            return Result.Failure(CardErrors.CardNotActive);
        }

        if (Pin.Hash != pin.Hash)
        {
            FailedPinAttempts++;

            RaiseDomainEvent(new PinAuthenticationFailedDomainEvent(Id, FailedPinAttempts));

            if (FailedPinAttempts >= MaxFailedPinAttempts)
            {
                Status = CardStatus.Confiscated;
                RaiseDomainEvent(new CardConfiscatedDomainEvent(Id));
                return Result.Failure(CardErrors.MaxFailedAttemptsReached);
            }

            return Result.Failure(CardErrors.InvalidPin);
        }

        FailedPinAttempts = 0;

        RaiseDomainEvent(new PinAuthenticatedDomainEvent(Id));

        return Result.Success();
    }

    public Result Block(string reason)
    {
        if (Status == CardStatus.Blocked)
        {
            return Result.Failure(CardErrors.AlreadyBlocked);
        }

        if (Status == CardStatus.Confiscated)
        {
            return Result.Failure(CardErrors.AlreadyConfiscated);
        }

        Status = CardStatus.Blocked;

        RaiseDomainEvent(new CardBlockedDomainEvent(Id, reason));

        return Result.Success();
    }

    public Result Confiscate()
    {
        if (Status == CardStatus.Confiscated)
        {
            return Result.Failure(CardErrors.AlreadyConfiscated);
        }

        if (Status == CardStatus.Blocked)
        {
            return Result.Failure(CardErrors.CardBlocked);
        }

        Status = CardStatus.Confiscated;

        RaiseDomainEvent(new CardConfiscatedDomainEvent(Id));

        return Result.Success();
    }
}
