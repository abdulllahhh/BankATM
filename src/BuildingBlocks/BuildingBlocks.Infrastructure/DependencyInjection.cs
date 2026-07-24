using BuildingBlocks.Application.Abstractions.Authentication;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Time;
using BuildingBlocks.Infrastructure.Authentication;
using BuildingBlocks.Infrastructure.Correlation;
using BuildingBlocks.Infrastructure.Dispatching;
using BuildingBlocks.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure;

/// <summary>
/// Registers reusable infrastructure services: current user resolution,
/// date/time, correlation context, and domain event dispatching.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
