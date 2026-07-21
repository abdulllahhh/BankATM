using Bank.Server.Application.Abstractions.Messaging;
using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Infrastructure.DomainEvent;
using Bank.Server.Infrastructure.Messaging;
using Bank.Server.Infrastructure.Persistence;
using Bank.Server.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Server.Infrastructure.DependencyInjection;

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
        });
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        //services.AddScoped<DomainEventsAccessor>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddScoped<ICardRepository, CardRepository>();

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddScoped<IATMRepository, ATMRepository>();

        return services;
    }
}