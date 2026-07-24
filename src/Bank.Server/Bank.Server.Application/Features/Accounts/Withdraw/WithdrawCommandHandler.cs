using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.ValueObjects;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Bank.Server.Application.Features.Accounts.Withdraw;

public class WithdrawCommandHandler
    : IRequestHandler<WithdrawCommand, Result>
{
    private readonly IAccountRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawCommandHandler(
        IAccountRepository repo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        WithdrawCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _repo.GetByAccountNumberAsync(
            AccountNumber.Create(request.AccountNumber),
            cancellationToken);

        if (account is null)
            return Result.Failure(new Error("ACCOUNT.NOT_FOUND", "Account not found"));

        var result = account.Withdraw(
            Money.Create(request.Amount, "USD"),
            request.AtmId);

        if (!result.IsSuccess)
            return Result.Failure(new Error("WITHDRAW.FAILED", result.Error));

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult;

        return Result.Success();
    }
}
