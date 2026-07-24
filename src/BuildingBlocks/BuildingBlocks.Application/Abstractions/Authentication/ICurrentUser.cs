namespace BuildingBlocks.Application.Abstractions.Authentication;

/// <summary>
/// Provides access to the current authenticated user's identity.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// The unique identifier of the current user, or null if unauthenticated.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Whether the current request is from an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }
}
