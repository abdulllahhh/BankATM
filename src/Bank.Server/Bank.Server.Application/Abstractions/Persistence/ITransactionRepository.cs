using Bank.Server.Domain.TransactionContext.Aggregates;

namespace Bank.Server.Application.Abstractions.Persistence;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct);
}