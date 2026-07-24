using Banking.Application.Abstractions.Persistence;
using Banking.Domain.Aggregates;
using BuildingBlocks.Application.Specifications;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Account>> ListAsync(ISpecification<Account> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<Account>(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Account?> FirstOrDefaultAsync(ISpecification<Account> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<Account>(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public void Remove(Account account)
    {
        _context.Accounts.Remove(account);
    }
}
