using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Application.Abstractions.Persistence;

/// <summary>
/// Provides write access to aggregate roots. Changes are tracked and committed by <see cref="IUnitOfWork"/>.
/// </summary>
public interface IRepository<TAggregate, TId> : IReadRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    /// <summary>
    /// Registers a new aggregate for insertion.
    /// </summary>
    void Add(TAggregate aggregate);

    /// <summary>
    /// Registers an existing aggregate for removal.
    /// </summary>
    void Remove(TAggregate aggregate);

    /// <summary>
    /// Registers an existing aggregate for update.
    /// </summary>
    void Update(TAggregate aggregate);
}
