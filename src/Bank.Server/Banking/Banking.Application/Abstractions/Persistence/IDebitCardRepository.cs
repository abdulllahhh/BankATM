using Banking.Domain.Cards.Aggregate;
using Banking.Domain.Cards.ValueObjects;
using BuildingBlocks.Application.Abstractions.Persistence;

namespace Banking.Application.Abstractions.Persistence;

public interface IDebitCardRepository : IRepository<DebitCard, Guid>
{
    Task<DebitCard?> GetByCardNumberAsync(
        CardNumber cardNumber,
        CancellationToken cancellationToken = default);
}
