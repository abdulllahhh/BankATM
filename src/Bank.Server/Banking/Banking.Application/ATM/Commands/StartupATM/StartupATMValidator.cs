using FluentValidation;

namespace Banking.Application.ATM.Commands.StartupATM;

public sealed class StartupATMCommandValidator : AbstractValidator<StartupATMCommand>
{
    public StartupATMCommandValidator()
    {
        RuleFor(x => x.ATMId)
            .NotEmpty()
            .WithMessage("ATM identifier is required.");
    }
}
