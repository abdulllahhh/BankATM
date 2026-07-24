using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Captures domain events from aggregates before save and dispatches them after save.
/// </summary>
public sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
}
