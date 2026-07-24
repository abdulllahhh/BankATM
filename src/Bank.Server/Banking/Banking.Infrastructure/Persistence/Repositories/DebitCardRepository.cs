using Banking.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;

namespace Banking.Infrastructure.Persistence.Repositories;

public sealed class DebitCardRepository : RepositoryBase<DebitCard, Guid>
{
    public DebitCardRepository(BankingDbContext dbContext)
    {
    }
}
