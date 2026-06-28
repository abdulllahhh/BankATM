using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.CardContext.Aggregates;
using Bank.Server.Domain.CardContext.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class CardRepository
    : ICardRepository
{
    private readonly BankDbContext _context;

    public CardRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Card?> GetByCardNumberAsync(
        CardNumber cardNumber,
        CancellationToken cancellationToken)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(
                x => x.CardNumber == cardNumber,
                cancellationToken);
    }
}