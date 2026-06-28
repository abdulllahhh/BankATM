using Bank.Server.Domain.ATMContext.Aggregates;
using Bank.Server.Domain.CardContext.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Infrastructure.Persistence.Configurations
{
    internal class ATMConfigurations : IEntityTypeConfiguration<ATM>
    {
        public void Configure(EntityTypeBuilder<ATM> builder)
        {
            builder.OwnsOne(
                x => x.ATMIdentifier,
                atmId =>
                {
                    atmId.Property(x => x.Value)
                         .HasColumnName("ATMIdentifier");
                });
            builder.ToTable("ATMs");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(
                x => x.CashAvailable,
                money =>
                {
                    money.Property(x => x.Amount)
                        .HasColumnName("CashAvailable")
                        .HasPrecision(18, 2);

                    money.Property(x => x.Currency)
                        .HasColumnName("Currency")
                        .HasMaxLength(3);
                });
        }
    }
}
