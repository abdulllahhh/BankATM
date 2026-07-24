using Banking.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal sealed class CashDispenserConfiguration : IEntityTypeConfiguration<CashDispenser>
{
    public void Configure(EntityTypeBuilder<CashDispenser> builder)
    {
        builder.ToTable("CashDispensers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ATMId)
            .IsRequired();

        builder.Property(c => c.Denomination)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Count)
            .IsRequired();

        builder.HasOne<ATM>()
            .WithMany()
            .HasForeignKey(c => c.ATMId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
