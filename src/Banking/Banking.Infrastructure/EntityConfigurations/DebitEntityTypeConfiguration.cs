using System;
using System.Transactions;
using Banking.Domain.Accounts;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.EntityConfigurations
{
    public class DebitEntityTypeConfiguration : IEntityTypeConfiguration<Debit>
    {
        public void Configure(EntityTypeBuilder<Debit> builder)
        {
            builder.ToTable("Debits", BankingDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            //Amount value object persisted as owned entity type supported since EF Core 2.0
            //builder.OwnsOne(o => o.Price);

            builder.Ignore(o => o.DomainEvents);
            //Amount value object persisted as owned entity type supported since EF Core 2.0
            builder.OwnsOne(o => o.Amount);

            builder.Property<long>("AccountId");
            builder.Property<DateTime>("CreatedAt");
            builder.Property<string>("Description").IsRequired().HasMaxLength(32);

            // builder.HasOne<Account>()
            //                           .WithMany()
            //                           .IsRequired(true)
            //                           .HasForeignKey("AccountId");

        }
    }
}