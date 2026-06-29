using BuildingBlocks.Application;

namespace Bank.Server.Application.Features.Accounts.GetBalance;

public sealed record GetBalanceQuery(
    string AccountNumber
) : IQuery<decimal>;