using Banking.Domain.Aggregates;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Persistence.Repositories;
using BuildingBlocks.Application.Abstractions.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBankingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BankingDbContext>(
            (_, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("BankingConnection"));
            });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BankingDbContext>());

        services.AddScoped<AuditInterceptor>();
        services.AddScoped<PublishDomainEventsInterceptor>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRepository<Account, Guid>, AccountRepository>();
        services.AddScoped<IRepository<DebitCard, Guid>, DebitCardRepository>();
        services.AddScoped<IRepository<ATM, Guid>, ATMRepository>();

        return services;
    }
}
