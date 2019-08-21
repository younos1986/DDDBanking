using Banking.Domain.Accounts;
using Banking.Domain.Customers;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.EntityConfigurations
{
    public class AccountStatusEntityTypeConfiguration : IEntityTypeConfiguration<AccountStatus>
    {
        public void Configure(EntityTypeBuilder<AccountStatus> builder)
        {
            builder.ToTable("AccountStatus", BankingDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();

            
           
        }
    }
}