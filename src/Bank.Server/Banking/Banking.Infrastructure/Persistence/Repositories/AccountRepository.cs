using Banking.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : RepositoryBase<Account, Guid>
{
    public AccountRepository(BankingDbContext dbContext)
    {
    }
}
