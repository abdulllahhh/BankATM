using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Specifications;
using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Infrastructure.Persistence.Repositories;

public class ReadRepository<TAggregate, TId> : IReadRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    public Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TAggregate>> ListAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate?> FirstOrDefaultAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
