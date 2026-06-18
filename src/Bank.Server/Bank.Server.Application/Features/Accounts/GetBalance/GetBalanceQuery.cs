using Bank.Server.Application.Common.Interfaces;

namespace Bank.Server.Application.Features.Accounts.GetBalance;

public sealed record GetBalanceQuery(
    string AccountNumber
) : IQuery<decimal>;