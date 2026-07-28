using Banking.Domain.ATM.ValueObjects;
using AggregateATM = Banking.Domain.ATM.Aggregate.ATM;

namespace Banking.Domain.ATM.Repositories;

public interface IATMRepository
{
    Task<AggregateATM?> GetByIdAsync(ATMId id, CancellationToken cancellationToken = default);
    Task AddAsync(AggregateATM atm, CancellationToken cancellationToken = default);
    void Update(AggregateATM atm);
    void Delete(AggregateATM atm);
}
