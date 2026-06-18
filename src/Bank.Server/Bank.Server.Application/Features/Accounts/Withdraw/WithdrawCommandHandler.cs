using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Application.Common.Models;
using Bank.Server.Domain.AccountContext.ValueObjects;
using MediatR;

namespace Bank.Server.Application.Features.Accounts.Withdraw;

public class WithdrawCommandHandler
    : IRequestHandler<WithdrawCommand, Result>
{
    private readonly IAccountRepository _repo;

    public WithdrawCommandHandler(IAccountRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result> Handle(
        WithdrawCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _repo.GetByAccountNumberAsync(
            AccountNumber.Create(request.AccountNumber),
            cancellationToken);

        if (account is null)
            return Result.Failure("Account not found");

        var result = account.Withdraw(
            Money.Create(request.Amount, "USD"));

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _repo.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}