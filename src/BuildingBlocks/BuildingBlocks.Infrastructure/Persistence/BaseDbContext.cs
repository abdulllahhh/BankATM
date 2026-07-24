using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base DbContext for all bounded contexts. Provides shared SaveChangesAsync
/// extensibility and a <see cref="ConfigureModel"/> hook that derived contexts
/// override instead of <see cref="OnModelCreating"/>.
///
/// Responsibilities are intentionally limited:
/// - Delegates to base SaveChangesAsync (no domain event dispatch, no auditing).
/// - Keeps the class infrastructure-agnostic (no references to MediatR, outbox, etc.).
/// - Derived contexts add their own DbSets, configurations, and interceptors.
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <summary>
    /// Saves changes to the database. Override this in derived classes to add
    /// pre-save or post-save behavior (e.g., interceptors, audit stamps).
    /// </summary>
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Reserved for future pre-save hooks (interceptors, audit, etc.).
        // Domain events are NOT dispatched here — they are handled by the
        // UnitOfWork or a dedicated pipeline behavior.

        var result = await base.SaveChangesAsync(cancellationToken);

        // Reserved for future post-save hooks.

        return result;
    }

    /// <summary>
    /// Configures the model after base initialization.
    /// Derived classes override this to apply entity configurations,
    /// rather than overriding <see cref="OnModelCreating"/> directly.
    /// </summary>
    protected virtual void ConfigureModel(ModelBuilder modelBuilder)
    {
    }

    protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureModel(modelBuilder);
    }
}
