using FluentValidation;

namespace Bank.Server.Application.Features.Accounts.Withdraw;

public class WithdrawCommandValidator
    : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}