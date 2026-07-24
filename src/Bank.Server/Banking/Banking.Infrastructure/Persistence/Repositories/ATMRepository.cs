using Banking.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class ATMRepository : RepositoryBase<ATM, Guid>
{
    public ATMRepository(BankingDbContext dbContext)
    {
    }
}
