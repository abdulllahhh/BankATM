using Banking.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal sealed class ATMTransactionConfiguration : IEntityTypeConfiguration<ATMTransaction>
{
    public void Configure(EntityTypeBuilder<ATMTransaction> builder)
    {
        builder.ToTable("ATMTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ATMId)
            .IsRequired();

        builder.Property(t => t.AccountId)
            .IsRequired();

        builder.Property(t => t.DebitCardId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .IsRequired();

        builder.Property(t => t.FailureReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne<ATM>()
            .WithMany()
            .HasForeignKey(t => t.ATMId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DebitCard>()
            .WithMany()
            .HasForeignKey(t => t.DebitCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
