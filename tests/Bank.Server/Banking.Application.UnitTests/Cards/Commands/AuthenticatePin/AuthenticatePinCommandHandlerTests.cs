using Banking.Application.Abstractions.Persistence;
using Banking.Application.Cards.Commands.AuthenticatePin;
using Banking.Domain.Cards.Aggregate;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Banking.Application.UnitTests.Cards.Commands.AuthenticatePin;

public sealed class AuthenticatePinCommandHandlerTests
{
    private readonly IDebitCardRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly AuthenticatePinCommandHandler _handler;

    private static readonly CardNumber ValidCardNumber = CardNumber.Create("4111111111111111");
    private static readonly Pin ValidPin = Pin.Create("1234");
    private static readonly Pin WrongPin = Pin.Create("5678");
    private static readonly IssueDate IssueDate = IssueDate.Create(
        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)));
    private static readonly ExpirationDate FutureExpirationDate = ExpirationDate.Create(
        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(3)), IssueDate);

    public AuthenticatePinCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IDebitCardRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _handler = new AuthenticatePinCommandHandler(_repositoryMock, _unitOfWorkMock);
    }

    private DebitCard CreateActiveCard()
        => DebitCard.Issue(ValidCardNumber, ValidPin, IssueDate, FutureExpirationDate);

    private void SetupRepositoryToReturn(DebitCard? card)
    {
        _repositoryMock
            .GetByCardNumberAsync(Arg.Any<CardNumber>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(card));
    }

    private void SetupSaveToSucceed()
    {
        _unitOfWorkMock
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));
    }

    [Fact]
    public async Task Handle_WhenCardNotFound_ReturnsFailureWithNotFoundError()
    {
        SetupRepositoryToReturn(null);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Card.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCardNotFound_DoesNotCallSaveChangesAsync()
    {
        SetupRepositoryToReturn(null);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAuthenticationSucceeds_ReturnsSuccess()
    {
        var card = CreateActiveCard();
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenAuthenticationSucceeds_CallsSaveChangesAsyncOnce()
    {
        var card = CreateActiveCard();
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAuthenticationSucceeds_ReturnsExpectedResponse()
    {
        var card = CreateActiveCard();
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CardId.Should().Be(card.Id);
        result.Value.IsAuthenticated.Should().BeTrue();
        result.Value.FailedAttempts.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenDomainResultFails_ReturnsFailureWithMappedError()
    {
        var card = CreateActiveCard();
        card.Block("Fraud");
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Card.CardBlocked);
    }

    [Fact]
    public async Task Handle_WhenDomainResultFails_CallsSaveChangesAsync()
    {
        var card = CreateActiveCard();
        card.Block("Fraud");
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPinIncorrect_CallsSaveChangesAsync()
    {
        var card = CreateActiveCard();
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "5678");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QueriesRepositoryExactlyOnce()
    {
        var card = CreateActiveCard();
        SetupRepositoryToReturn(card);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        await _handler.Handle(command, CancellationToken.None);

        await _repositoryMock.Received(1).GetByCardNumberAsync(
             Arg.Any<CardNumber>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExpiredCard_ReturnsExpiredError()
    {
        var pastIssueDate = IssueDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)));
        var pastExpirationDate = ExpirationDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)), pastIssueDate);
        var expiredCard = DebitCard.Issue(ValidCardNumber, ValidPin, pastIssueDate, pastExpirationDate);
        SetupRepositoryToReturn(expiredCard);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Errors.Card.CardExpired);
    }

    [Fact]
    public async Task Handle_WhenExpiredCard_SavesChanges()
    {
        var pastIssueDate = IssueDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)));
        var pastExpirationDate = ExpirationDate.Create(
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)), pastIssueDate);
        var expiredCard = DebitCard.Issue(ValidCardNumber, ValidPin, pastIssueDate, pastExpirationDate);
        SetupRepositoryToReturn(expiredCard);
        SetupSaveToSucceed();
        var command = new AuthenticatePinCommand("4111111111111111", "1234");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
