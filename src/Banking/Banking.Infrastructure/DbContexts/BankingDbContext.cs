using System;
using System.Collections.Generic;
using System.Data;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Accounts;
using Banking.Domain.Customers;
using Banking.Domain.IntegrationEvents;
using Banking.Domain.SeedWorks;
using Banking.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Banking.Infrastructure.DbContexts
{
    public class BankingDbContext : DbContext , IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "banking";
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountStatus> AccountStatuses { get; set; }
        public virtual DbSet<Debit> Debits { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; }

        
       


        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;
        public BankingDbContext(DbContextOptions<BankingDbContext> options, IMediator mediator )
                    : base(options)
        {
             _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new AccountEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AccountStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CreditEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DebitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationEventLogEntityTypeConfiguration());
            


            modelBuilder.Entity<AccountStatus>().HasData(
                new List<AccountStatus>(){
                    new AccountStatus(1 , "Opened"),
                    new AccountStatus( 2, "Locked" ),
                    new AccountStatus( 3, "Closed" )
            }.ToArray());

        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

       

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

       
    }
}