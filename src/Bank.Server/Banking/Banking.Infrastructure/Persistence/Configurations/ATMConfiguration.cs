using Banking.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal sealed class ATMConfiguration : IEntityTypeConfiguration<ATM>
{
    public void Configure(EntityTypeBuilder<ATM> builder)
    {
        builder.ToTable("ATMs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Identifier)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => a.Identifier)
            .IsUnique();

        builder.Property(a => a.Location)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.LastMaintenance)
            .IsRequired(false);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
