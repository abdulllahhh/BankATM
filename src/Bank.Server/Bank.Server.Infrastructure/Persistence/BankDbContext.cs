
using Bank.Server.Domain.TransactionContext.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Infrastructure.Persistence;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options) { }

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}