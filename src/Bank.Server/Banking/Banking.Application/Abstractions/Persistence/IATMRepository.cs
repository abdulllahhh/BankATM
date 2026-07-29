using Banking.Domain.ATM.ValueObjects;
using ATMAggregate = Banking.Domain.ATM.Aggregate.ATM;
using BuildingBlocks.Application.Abstractions.Persistence;

namespace Banking.Application.Abstractions.Persistence;

public interface IATMRepository : IRepository<ATMAggregate, ATMId>
{
}
