using System.Security.Claims;
using BuildingBlocks.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure.Authentication;

/// <summary>
/// Resolves the current authenticated user's identity from the ASP.NET Core
/// <see cref="IHttpContextAccessor"/>. Returns <c>null</c> for
/// <see cref="ICurrentUser.UserId"/> when the request is unauthenticated.
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return null;
            }

            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return value is not null ? Guid.Parse(value) : null;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }
}
