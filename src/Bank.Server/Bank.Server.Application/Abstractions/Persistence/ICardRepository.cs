using Bank.Server.Domain.CardContext.ValueObjects;

namespace Bank.Server.Application.Abstractions.Persistence;

public interface ICardRepository
{
    Task<Card?> GetByCardNumberAsync(
        CardNumber cardNumber,
        CancellationToken cancellationToken);
}