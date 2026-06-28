using Bank.Server.Domain.ATMContext.Aggregates;

namespace Bank.Server.Application.Abstractions.Persistence;

public interface IATMRepository
{
    Task<ATM?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}