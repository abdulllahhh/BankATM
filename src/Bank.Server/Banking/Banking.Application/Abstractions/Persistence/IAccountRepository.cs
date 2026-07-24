using Banking.Domain.Aggregates;
using BuildingBlocks.Application.Abstractions.Persistence;

namespace Banking.Application.Abstractions.Persistence;

public interface IAccountRepository : IRepository<Account, Guid>
{
}
