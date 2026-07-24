using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Specifications;

namespace BuildingBlocks.Application.Abstractions.Persistence;

/// <summary>
/// Provides read-only access to aggregate roots for query operations.
/// </summary>
public interface IReadRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    /// <summary>
    /// Retrieves an aggregate by its unique identifier.
    /// </summary>
    Task<TAggregate?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether an aggregate with the given identifier exists.
    /// </summary>
    Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all aggregates matching the specification.
    /// </summary>
    Task<List<TAggregate>> ListAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the first aggregate matching the specification, or null if none match.
    /// </summary>
    Task<TAggregate?> FirstOrDefaultAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default);
}
