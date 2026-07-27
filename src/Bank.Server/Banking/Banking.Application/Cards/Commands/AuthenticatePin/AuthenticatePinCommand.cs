using BuildingBlocks.Application.CQRS;

namespace Banking.Application.Cards.Commands.AuthenticatePin;

public sealed record AuthenticatePinCommand(
    string CardNumber,
    string Pin)
    : ICommand<AuthenticatePinResponse>;
