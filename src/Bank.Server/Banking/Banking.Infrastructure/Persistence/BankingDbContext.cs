using Banking.Domain.Aggregates;
using CardAggregate = Banking.Domain.Cards.Aggregate;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : BaseDbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CardAggregate.DebitCard> DebitCards => Set<CardAggregate.DebitCard>();
    public DbSet<ATM> ATMs => Set<ATM>();
    public DbSet<CashDispenser> CashDispensers => Set<CashDispenser>();
    public DbSet<ATMTransaction> ATMTransactions => Set<ATMTransaction>();

    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }
}
