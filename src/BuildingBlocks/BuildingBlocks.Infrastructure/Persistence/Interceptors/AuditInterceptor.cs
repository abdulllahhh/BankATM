using BuildingBlocks.Application.Abstractions.Authentication;
using BuildingBlocks.Application.Abstractions.Time;
using BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically populates audit metadata on entities implementing
/// <see cref="IAuditableEntity"/> before they are saved to the database.
///
/// For new entities: sets <see cref="IAuditableEntity.CreatedAt"/>
/// and optionally <see cref="IAuditableEntity.CreatedBy"/>.
///
/// For modified entities: sets <see cref="IAuditableEntity.ModifiedAt"/>
/// and optionally <see cref="IAuditableEntity.ModifiedBy"/>.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUser? _currentUser;

    public AuditInterceptor(
        IDateTimeProvider dateTimeProvider,
        ICurrentUser? currentUser = null)
    {
        _dateTimeProvider = dateTimeProvider;
        _currentUser = currentUser;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var now = _dateTimeProvider.UtcNow;
        var userId = _currentUser?.UserId?.ToString();

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = now;
                    if (userId is not null)
                    {
                        entry.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = userId;
                    }
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.ModifiedAt)).CurrentValue = now;
                    if (userId is not null)
                    {
                        entry.Property(nameof(IAuditableEntity.ModifiedBy)).CurrentValue = userId;
                    }
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
