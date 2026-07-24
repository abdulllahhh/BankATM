
using BuildingBlocks.Application;

namespace Bank.Server.Application.Features.Accounts.Withdraw;

public sealed record WithdrawCommand(
    string AccountNumber,
    Guid AtmId,
    decimal Amount
) : ICommand;
