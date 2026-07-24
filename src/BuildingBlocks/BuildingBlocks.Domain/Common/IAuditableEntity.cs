namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Marks an entity as supporting audit tracking.
/// The <see cref="BuildingBlocks.Infrastructure.Persistence.Interceptors.AuditInterceptor"/>
/// populates these properties automatically before saving changes.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime? ModifiedAt { get; }
    string? CreatedBy { get; }
    string? ModifiedBy { get; }
}
