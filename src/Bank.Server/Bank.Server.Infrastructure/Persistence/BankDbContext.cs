
using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Domain.ATMContext.Aggregates;
using Bank.Server.Domain.AuditContext.Aggregates;
using Bank.Server.Domain.CardContext.Aggregates;
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
    public DbSet<ATM> ATMs => Set<ATM>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}