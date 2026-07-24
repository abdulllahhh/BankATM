using BuildingBlocks.Application.Abstractions.Authentication;

namespace BuildingBlocks.Infrastructure.Authentication;

/// <summary>
/// Provides the current authenticated user identity. Designed to be scoped and populated by infrastructure.
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    public Guid? UserId => null;

    public bool IsAuthenticated => false;
}
