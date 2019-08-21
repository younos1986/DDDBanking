using Banking.Domain.Accounts;
using Banking.Domain.Customers;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.EntityConfigurations
{
    public class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts", BankingDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("accountSeq", BankingDbContext.DEFAULT_SCHEMA);
            //builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Ignore(b => b.DomainEvents);
            // builder.Ignore(b => b.AccountStatus);


            builder.Property<long>("CustomerId").IsRequired();
            builder.Property<int>("AccountStatusId").IsRequired(); 
            builder.Property<string>("Description").HasMaxLength(256);
            
            builder.HasOne<Customer>()
               .WithMany()
               .IsRequired(true)
               .HasForeignKey("CustomerId");

            
           
        }
    }
}