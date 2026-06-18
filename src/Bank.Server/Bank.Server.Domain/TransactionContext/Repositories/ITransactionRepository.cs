using Bank.Server.Domain.TransactionContext.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.TransactionContext.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(
            Transaction transaction,
            CancellationToken cancellationToken);

        Task<Transaction?> GetByIdAsync(
            Guid transactionId,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
