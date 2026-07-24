using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Base DbContext for all bounded contexts in the system.
///
/// Automatically discovers and applies <see cref="IEntityTypeConfiguration{TEntity}"/>
/// classes from the assembly of the derived context via
/// <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>,
/// so each module only needs to define its own configurations and DbSets.
///
/// Responsibilities are intentionally minimal:
/// - Accepts <see cref="DbContextOptions"/> and passes them to the base class.
/// - Overrides <see cref="SaveChangesAsync(CancellationToken)"/> as a thin
///   pass-through to the base implementation.
/// - Overrides <see cref="OnModelCreating(ModelBuilder)"/> (as sealed) to
///   automatically scan the derived assembly for entity configurations.
///
/// Derived contexts MUST NOT override <see cref="OnModelCreating"/>.
/// All entity configuration must be done via
/// <see cref="IEntityTypeConfiguration{TEntity}"/> classes.
/// Cross-cutting concerns (auditing, domain events, soft deletes, etc.) are
/// handled by registered EF Core interceptors, not by this class.
/// </summary>
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
