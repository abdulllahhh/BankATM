using Banking.Application.Abstractions.Persistence;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Persistence.Repositories;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure;

/// <summary>
/// Registers the Banking bounded context infrastructure: DbContext with
/// interceptors, UnitOfWork, and aggregate repositories.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBankingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<PublishDomainEventsInterceptor>();

        services.AddDbContext<BankingDbContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
            var domainEventsInterceptor = sp.GetRequiredService<PublishDomainEventsInterceptor>();

            options.UseNpgsql(configuration.GetConnectionString("BankingConnection"))
                   .AddInterceptors(auditInterceptor, domainEventsInterceptor);
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BankingDbContext>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IDebitCardRepository, DebitCardRepository>();
        services.AddScoped<IATMRepository, ATMRepository>();

        return services;
    }
}
