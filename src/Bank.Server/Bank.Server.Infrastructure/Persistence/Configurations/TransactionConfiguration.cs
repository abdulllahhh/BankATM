using Bank.Server.Domain.TransactionContext.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Server.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .IsRequired();
        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.FromAccountId)
            .IsRequired(false);

        builder.Property(t => t.ToAccountId)
            .IsRequired(false);

        builder.OwnsOne(
            x => x.Amount,
            money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2);

                money.Property(x => x.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3);
            });

    }
}