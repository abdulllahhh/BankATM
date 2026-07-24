using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Application.Specifications;
using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base class for full aggregate repositories combining read and write operations.
/// </summary>
public abstract class RepositoryBase<TAggregate, TId> : IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    public void Add(TAggregate aggregate)
    {
        throw new NotImplementedException();
    }

    public void Remove(TAggregate aggregate)
    {
        throw new NotImplementedException();
    }

    public void Update(TAggregate aggregate)
    {
        throw new NotImplementedException();
    }

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
