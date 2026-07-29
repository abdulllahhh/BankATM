using ATMAggregate = Banking.Domain.ATM.Aggregate.ATM;
using Banking.Application.Abstractions.Persistence;
using Banking.Domain.ATM.ValueObjects;
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

    public async Task<ATMAggregate?> GetByIdAsync(ATMId id, CancellationToken cancellationToken = default)
    {
        return await _context.ATMs
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(ATMId id, CancellationToken cancellationToken = default)
    {
        return await _context.ATMs.AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<ATMAggregate>> ListAsync(ISpecification<ATMAggregate> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<ATMAggregate>(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ATMAggregate?> FirstOrDefaultAsync(ISpecification<ATMAggregate> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<ATMAggregate>(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(ATMAggregate atm)
    {
        _context.ATMs.Add(atm);
    }

    public void Update(ATMAggregate atm)
    {
        _context.ATMs.Update(atm);
    }

    public void Remove(ATMAggregate atm)
    {
        _context.ATMs.Remove(atm);
    }
}
