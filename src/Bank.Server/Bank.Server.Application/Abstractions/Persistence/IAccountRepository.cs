using Bank.Server.Domain.AccountContext.ValueObjects;

namespace Bank.Server.Application.Abstractions.Persistence;

public interface IAccountRepository
{
    Task<Account?> GetByAccountNumberAsync(
        AccountNumber accountNumber,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}