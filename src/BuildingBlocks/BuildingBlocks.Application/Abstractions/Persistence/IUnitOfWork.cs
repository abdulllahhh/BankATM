using BuildingBlocks.Application.Results;

namespace BuildingBlocks.Application.Abstractions.Persistence;

/// <summary>
/// Commits a single business transaction by persisting all tracked changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes within the current transaction.
    /// </summary>
    Task<Result> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
