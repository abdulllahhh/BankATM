using Bank.Server.Infrastructure.DomainEvent;
using Bank.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BankDbContext>(
        (sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            options.AddInterceptors(
                sp.GetRequiredService<
                    PublishDomainEventsInterceptor>());
        });
        services.AddScoped<PublishDomainEventsInterceptor>();

        return services;
    }
}