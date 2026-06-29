
using BuildingBlocks.Application;

namespace Bank.Server.Application.Features.Accounts.Withdraw;

public sealed record WithdrawCommand(
    string AccountNumber,
    decimal Amount
) : ICommand;