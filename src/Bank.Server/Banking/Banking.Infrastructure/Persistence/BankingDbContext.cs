using ATMAggregate = Banking.Domain.ATM.Aggregate.ATM;
using CardAggregate = Banking.Domain.Cards.Aggregate;
using OldAccount = Banking.Domain.Aggregates.Account;
using OldCashDispenser = Banking.Domain.Aggregates.CashDispenser;
using OldATMTransaction = Banking.Domain.Aggregates.ATMTransaction;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : BaseDbContext
{
    public DbSet<OldAccount> Accounts => Set<OldAccount>();
    public DbSet<CardAggregate.DebitCard> DebitCards => Set<CardAggregate.DebitCard>();
    public DbSet<ATMAggregate> ATMs => Set<ATMAggregate>();
    public DbSet<OldCashDispenser> CashDispensers => Set<OldCashDispenser>();
    public DbSet<OldATMTransaction> ATMTransactions => Set<OldATMTransaction>();

    public BankingDbContext(DbContextOptions<BankingDbContext> options)
        : base(options)
    {
    }
}
