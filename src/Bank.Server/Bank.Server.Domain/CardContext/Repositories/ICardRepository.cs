using Bank.Server.Domain.CardContext.Aggregates;
using Bank.Server.Domain.CardContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.CardContext.Repositories
{
    public interface ICardRepository
    {
        Task<Card?> GetByCardNumberAsync(
            CardNumber cardNumber,
            CancellationToken cancellationToken);

        Task AddAsync(
            Card card,
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
