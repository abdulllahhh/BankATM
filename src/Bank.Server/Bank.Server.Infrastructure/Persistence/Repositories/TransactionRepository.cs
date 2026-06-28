using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.TransactionContext.Aggregates;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository
    : ITransactionRepository
{
    private readonly BankDbContext _context;

    public TransactionRepository(
        BankDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken)
    {
        await _context.Transactions.AddAsync(
            transaction,
            cancellationToken);
    }
}