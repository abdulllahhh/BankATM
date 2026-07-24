using Banking.Domain.Aggregates;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : BaseDbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<DebitCard> DebitCards => Set<DebitCard>();
    public DbSet<ATM> ATMs => Set<ATM>();
    public DbSet<CashDispenser> CashDispensers => Set<CashDispenser>();
    public DbSet<ATMTransaction> ATMTransactions => Set<ATMTransaction>();

    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }

    protected override void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);
    }
}
