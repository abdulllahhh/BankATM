using BuildingBlocks.Application.Abstractions.Authentication;

namespace BuildingBlocks.Infrastructure.Persistence.Services;

public class CurrentUserService : ICurrentUser
{
    public Guid? UserId => null;

    public bool IsAuthenticated => false;
}
