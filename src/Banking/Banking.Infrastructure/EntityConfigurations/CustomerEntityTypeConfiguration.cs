using System;
using System.Collections.Generic;
using Banking.Domain.Customers;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.EntityConfigurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

            builder.ToTable("Customers", BankingDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Ignore(b => b.DomainEvents);
            
            builder.Property<string>("FirstName").IsRequired().HasMaxLength(32);
            builder.Property<string>("LastName").IsRequired().HasMaxLength(32);
            builder.Property<DateTime>("BirthDay");

            //builder.Ignore(b => b.Accounts);
            


        }
    }
}