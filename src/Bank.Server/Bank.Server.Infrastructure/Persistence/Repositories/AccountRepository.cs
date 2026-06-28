using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Domain.AccountContext.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository
    : IAccountRepository
{
    private readonly BankDbContext _context;

    public AccountRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByAccountNumberAsync(
        AccountNumber accountNumber,
        CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(
                x => x.AccountNumber == accountNumber,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}