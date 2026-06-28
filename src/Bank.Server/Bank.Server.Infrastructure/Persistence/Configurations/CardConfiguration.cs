using Bank.Server.Domain.CardContext.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Server.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(c => c.Id);
        builder.OwnsOne(
            x => x.CardNumber,
            cardNumber =>
            {
                cardNumber.Property(x => x.Value)
                    .HasColumnName("CardNumber")
                    .HasMaxLength(20)
                    .IsRequired();

                cardNumber.HasIndex(x => x.Value)
                           .IsUnique();
            });

        builder.Property(c => c.PinHash)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.FailedPinAttempts)
            .IsRequired();

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.ExpiryDate)
            .IsRequired();
    }
}