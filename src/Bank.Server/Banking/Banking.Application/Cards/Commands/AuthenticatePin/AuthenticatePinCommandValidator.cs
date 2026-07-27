using FluentValidation;

namespace Banking.Application.Cards.Commands.AuthenticatePin;

public sealed class AuthenticatePinCommandValidator
    : AbstractValidator<AuthenticatePinCommand>
{
    private const int MinPinLength = 4;
    private const int MaxPinLength = 6;

    public AuthenticatePinCommandValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Must(BeValidCardNumber)
            .WithMessage("Card number must contain only digits.");

        RuleFor(x => x.Pin)
            .NotEmpty()
            .Must(BeValidPin)
            .WithMessage($"PIN must be between {MinPinLength} and {MaxPinLength} digits.");
    }

    private static bool BeValidCardNumber(string value)
    {
        return value.All(char.IsDigit);
    }

    private static bool BeValidPin(string value)
    {
        return value.All(char.IsDigit)
            && value.Length >= MinPinLength
            && value.Length <= MaxPinLength;
    }
}
