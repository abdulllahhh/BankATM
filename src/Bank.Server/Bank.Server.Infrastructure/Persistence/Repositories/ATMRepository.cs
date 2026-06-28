using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.ATMContext.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class ATMRepository
    : IATMRepository
{
    private readonly BankDbContext _context;

    public ATMRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<ATM?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.ATMs
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}