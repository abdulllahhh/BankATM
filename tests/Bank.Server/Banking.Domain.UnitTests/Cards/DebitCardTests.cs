using Banking.Domain.Cards.Aggregate;
using Banking.Domain.Cards.Enums;
using Banking.Domain.Cards.Errors;
using Banking.Domain.Cards.Events;
using Banking.Domain.Cards.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Banking.Domain.UnitTests.Cards;

public sealed class DebitCardTests
{
    private static readonly CardNumber ValidCardNumber = CardNumber.Create("4111111111111111");
    private static readonly Pin ValidPin = Pin.Create("1234");
    private static readonly Pin WrongPin = Pin.Create("5678");
    private static readonly IssueDate IssueDate = IssueDate.Create(
        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)));
    private static readonly ExpirationDate FutureExpirationDate = ExpirationDate.Create(
        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(3)), IssueDate);

    private static DebitCard CreateActiveCard()
        => DebitCard.Issue(ValidCardNumber, ValidPin, IssueDate, FutureExpirationDate);

    [Fact]
    public void AuthenticatePin_WithCorrectPin_ReturnsSuccess()
    {
        var card = CreateActiveCard();

        var result = card.AuthenticatePin(ValidPin);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AuthenticatePin_WithCorrectPin_ResetsFailedAttemptsToZero()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);

        card.AuthenticatePin(ValidPin);

        card.FailedPinAttempts.Should().Be(0);
    }

    [Fact]
    public void AuthenticatePin_WithCorrectPin_RaisesPinAuthenticatedDomainEvent()
    {
        var card = CreateActiveCard();

        card.AuthenticatePin(ValidPin);

        card.DomainEvents.Should().ContainSingle(e => e is PinAuthenticatedDomainEvent);
    }

    [Fact]
    public void AuthenticatePin_WithIncorrectPin_ReturnsFailure()
    {
        var card = CreateActiveCard();

        var result = card.AuthenticatePin(WrongPin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CardErrors.InvalidPin);
    }

    [Fact]
    public void AuthenticatePin_WithIncorrectPin_IncrementsFailedAttempts()
    {
        var card = CreateActiveCard();

        card.AuthenticatePin(WrongPin);

        card.FailedPinAttempts.Should().Be(1);
    }

    [Fact]
    public void AuthenticatePin_WithIncorrectPin_RaisesPinAuthenticationFailedDomainEvent()
    {
        var card = CreateActiveCard();

        card.AuthenticatePin(WrongPin);

        var failedEvent = card.DomainEvents.Should()
            .ContainSingle(e => e is PinAuthenticationFailedDomainEvent).Which
            as PinAuthenticationFailedDomainEvent;
        failedEvent!.FailedAttempts.Should().Be(1);
    }

    [Fact]
    public void AuthenticatePin_AfterThreeFailedAttempts_ConfiscatesCard()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.AuthenticatePin(WrongPin);

        card.AuthenticatePin(WrongPin);

        card.Status.Should().Be(CardStatus.Confiscated);
    }

    [Fact]
    public void AuthenticatePin_AfterThreeFailedAttempts_ReturnsMaxFailedAttemptsError()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.AuthenticatePin(WrongPin);

        var result = card.AuthenticatePin(WrongPin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CardErrors.MaxFailedAttemptsReached);
    }

    [Fact]
    public void AuthenticatePin_AfterThreeFailedAttempts_RaisesCardConfiscatedDomainEvent()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.AuthenticatePin(WrongPin);

        card.AuthenticatePin(WrongPin);

        card.DomainEvents.Should().Contain(e => e is CardConfiscatedDomainEvent);
    }

    [Fact]
    public void AuthenticatePin_AfterThreeFailedAttempts_RaisesPinAuthenticationFailedForEachAttempt()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.AuthenticatePin(WrongPin);

        card.AuthenticatePin(WrongPin);

        var failedEvents = card.DomainEvents
            .OfType<PinAuthenticationFailedDomainEvent>().ToList();
        failedEvents.Should().HaveCount(3);
        failedEvents[0].FailedAttempts.Should().Be(1);
        failedEvents[1].FailedAttempts.Should().Be(2);
        failedEvents[2].FailedAttempts.Should().Be(3);
    }

    [Fact]
    public void AuthenticatePin_OnExpiredCard_ReturnsExpiredError()
    {
        var pastIssueDate = IssueDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)));
        var pastExpirationDate = ExpirationDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)), pastIssueDate);
        var card = DebitCard.Issue(ValidCardNumber, ValidPin, pastIssueDate, pastExpirationDate);

        var result = card.AuthenticatePin(ValidPin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CardErrors.CardExpired);
    }

    [Fact]
    public void AuthenticatePin_OnExpiredCard_SetsStatusToExpired()
    {
        var pastIssueDate = IssueDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)));
        var pastExpirationDate = ExpirationDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)), pastIssueDate);
        var card = DebitCard.Issue(ValidCardNumber, ValidPin, pastIssueDate, pastExpirationDate);

        card.AuthenticatePin(ValidPin);

        card.Status.Should().Be(CardStatus.Expired);
    }

    [Fact]
    public void AuthenticatePin_OnBlockedCard_ReturnsBlockedError()
    {
        var card = CreateActiveCard();
        card.Block("Lost card");

        var result = card.AuthenticatePin(ValidPin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CardErrors.CardBlocked);
    }

    [Fact]
    public void AuthenticatePin_OnBlockedCard_DoesNotChangeStatus()
    {
        var card = CreateActiveCard();
        card.Block("Lost card");
        card.ClearDomainEvents();

        card.AuthenticatePin(ValidPin);

        card.Status.Should().Be(CardStatus.Blocked);
    }

    [Fact]
    public void AuthenticatePin_OnConfiscatedCard_ReturnsConfiscatedError()
    {
        var card = CreateActiveCard();
        card.Confiscate();

        var result = card.AuthenticatePin(ValidPin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CardErrors.CardConfiscated);
    }

    [Fact]
    public void AuthenticatePin_OnConfiscatedCard_DoesNotChangeStatus()
    {
        var card = CreateActiveCard();
        card.Confiscate();
        card.ClearDomainEvents();

        card.AuthenticatePin(ValidPin);

        card.Status.Should().Be(CardStatus.Confiscated);
    }

    [Fact]
    public void AuthenticatePin_AfterSuccessfulAuthentication_NoDomainEventFromPreviousFailureRemains()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.ClearDomainEvents();

        card.AuthenticatePin(ValidPin);

        card.DomainEvents.Should().ContainSingle(e => e is PinAuthenticatedDomainEvent);
        card.DomainEvents.Should().NotContain(e => e is PinAuthenticationFailedDomainEvent);
    }

    [Fact]
    public void AuthenticatePin_WithTwoFailedAttemptsThenCorrectPin_ReturnsSuccess()
    {
        var card = CreateActiveCard();
        card.AuthenticatePin(WrongPin);
        card.AuthenticatePin(WrongPin);

        var result = card.AuthenticatePin(ValidPin);

        result.IsSuccess.Should().BeTrue();
        card.FailedPinAttempts.Should().Be(0);
    }
}
