using Banking.Application.Abstractions.Persistence;
using Banking.Domain.Aggregates;
using BuildingBlocks.Application.Specifications;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class ATMRepository : IATMRepository
{
    private readonly BankingDbContext _context;

    public ATMRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<ATM?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ATMs
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ATMs.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<ATM>> ListAsync(ISpecification<ATM> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<ATM>(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ATM?> FirstOrDefaultAsync(ISpecification<ATM> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<ATM>(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(ATM atm)
    {
        _context.ATMs.Add(atm);
    }

    public void Update(ATM atm)
    {
        _context.ATMs.Update(atm);
    }

    public void Remove(ATM atm)
    {
        _context.ATMs.Remove(atm);
    }
}
