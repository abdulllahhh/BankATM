using Banking.Application.Abstractions.Persistence;
using Banking.Domain.Cards.Errors;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Banking.Application.Cards.Commands.AuthenticatePin;

internal sealed class AuthenticatePinCommandHandler
    : IRequestHandler<AuthenticatePinCommand, Result<AuthenticatePinResponse>>
{
    private readonly IDebitCardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticatePinCommandHandler(
        IDebitCardRepository cardRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthenticatePinResponse>> Handle(
        AuthenticatePinCommand request,
        CancellationToken cancellationToken)
    {
        var cardNumber = CardNumber.Create(request.CardNumber);
        var pin = Pin.Create(request.Pin);

        var card = await _cardRepository.GetByCardNumberAsync(
            cardNumber,
            cancellationToken);

        if (card is null)
        {
            return Result<AuthenticatePinResponse>.Failure(Errors.Card.NotFound);
        }

        var domainResult = card.AuthenticatePin(pin);

        if (domainResult.IsFailure)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var error = MapError(domainResult.Error);
            return Result<AuthenticatePinResponse>.Failure(error);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return Result<AuthenticatePinResponse>.Failure(saveResult.Error);
        }

        var response = new AuthenticatePinResponse(
            card.Id,
            MaskCardNumber(cardNumber.Value),
            domainResult.IsSuccess,
            card.FailedPinAttempts);

        return Result<AuthenticatePinResponse>.Success(response);
    }

    private static Error MapError(string domainError)
    {
        return domainError switch
        {
            CardErrors.CardNotActive => Errors.Card.CardNotActive,
            CardErrors.CardBlocked => Errors.Card.CardBlocked,
            CardErrors.CardConfiscated => Errors.Card.CardConfiscated,
            CardErrors.CardExpired => Errors.Card.CardExpired,
            CardErrors.InvalidPin => Errors.Card.InvalidPin,
            CardErrors.MaxFailedAttemptsReached => Errors.Card.MaxFailedAttempts,
            _ => Errors.General.Unexpected
        };
    }

    private static string MaskCardNumber(string cardNumber)
    {
        var visible = cardNumber.Length >= 4
            ? cardNumber[^4..]
            : cardNumber;

        return $"****{visible}";
    }
}
