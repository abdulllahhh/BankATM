using Banking.Domain.Cards.Aggregate;
using Banking.Domain.Cards.Enums;
using Banking.Domain.Cards.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal sealed class DebitCardConfiguration : IEntityTypeConfiguration<DebitCard>
{
    public void Configure(EntityTypeBuilder<DebitCard> builder)
    {
        builder.ToTable("DebitCards");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.CardNumber)
            .HasConversion(cn => cn.Value, v => CardNumber.Create(v))
            .HasMaxLength(19)
            .IsRequired();

        builder.HasIndex(d => d.CardNumber)
            .IsUnique();

        builder.Property(d => d.Pin)
            .HasConversion(p => p.Hash, v => Pin.FromHash(v))
            .HasMaxLength(64)
            .IsRequired();

        builder.ComplexProperty(d => d.IssueDate, b =>
        {
            b.Property(id => id.Value)
                .HasColumnName("IssueDate")
                .IsRequired();
        });

        builder.ComplexProperty(d => d.ExpirationDate, b =>
        {
            b.Property(ed => ed.Value)
                .HasColumnName("ExpirationDate")
                .IsRequired();
        });

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.FailedPinAttempts)
            .IsRequired();
    }
}
