using Banking.Application.Abstractions.Persistence;
using Banking.Domain.Cards.Aggregate;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Application.Specifications;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class DebitCardRepository : IDebitCardRepository
{
    private readonly BankingDbContext _context;

    public DebitCardRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<DebitCard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DebitCards
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DebitCard?> GetByCardNumberAsync(
        CardNumber cardNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.DebitCards
            .FirstOrDefaultAsync(d => d.CardNumber == cardNumber, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DebitCards.AnyAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<List<DebitCard>> ListAsync(ISpecification<DebitCard> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<DebitCard>(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<DebitCard?> FirstOrDefaultAsync(ISpecification<DebitCard> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Set<DebitCard>(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(DebitCard debitCard)
    {
        _context.DebitCards.Add(debitCard);
    }

    public void Update(DebitCard debitCard)
    {
        _context.DebitCards.Update(debitCard);
    }

    public void Remove(DebitCard debitCard)
    {
        _context.DebitCards.Remove(debitCard);
    }
}
