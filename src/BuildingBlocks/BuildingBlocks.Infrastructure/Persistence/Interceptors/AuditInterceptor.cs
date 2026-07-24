using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically sets audit timestamps on entities implementing audit interfaces.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
}
