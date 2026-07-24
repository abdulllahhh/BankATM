using Banking.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal sealed class DebitCardConfiguration : IEntityTypeConfiguration<DebitCard>
{
    public void Configure(EntityTypeBuilder<DebitCard> builder)
    {
        builder.ToTable("DebitCards");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.AccountId)
            .IsRequired();

        builder.Property(d => d.CardNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(d => d.CardNumber)
            .IsUnique();

        builder.Property(d => d.ExpiryDate)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
