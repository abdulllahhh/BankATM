namespace Banking.Application.Cards.Commands.AuthenticatePin;

public sealed record AuthenticatePinResponse(
    Guid CardId,
    string MaskedCardNumber,
    bool IsAuthenticated,
    int FailedAttempts);
