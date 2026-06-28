using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Domain.AccountContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        // VALUE OBJECT mapping (ONLY ONCE)
        builder.Property(x => x.AccountNumber)
            .HasConversion(
                x => x.Value,
                x => AccountNumber.Create(x))
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex("AccountNumber")
            .IsUnique();

        // Money
        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(x => x.Amount)
                .HasColumnName("Balance")
                .HasPrecision(18, 2);

            balance.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.DailyLimit, limit =>
        {
            limit.Property(x => x.Amount)
                .HasColumnName("DailyLimit")
                .HasPrecision(18, 2);

            limit.Property(x => x.Currency)
                .HasColumnName("DailyLimitCurrency");
        });

        builder.OwnsOne(x => x.WithdrawnToday, withdrawn =>
        {
            withdrawn.Property(x => x.Amount)
                .HasColumnName("WithdrawnToday")
                .HasPrecision(18, 2);

            withdrawn.Property(x => x.Currency)
                .HasColumnName("WithdrawnTodayCurrency");
        });

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}