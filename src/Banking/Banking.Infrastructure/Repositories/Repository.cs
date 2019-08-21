using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Banking.Domain;
using Banking.Domain.SeedWorks;
using Banking.Infrastructure.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Banking.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly BankingDbContext _dbContext;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _dbContext;
            }
        }

        public Repository(BankingDbContext dbContext
        )
        {
            _dbContext = dbContext;

        }

        


        public virtual async Task<T> CreateAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            await _dbContext.Set<T>().AddAsync(item);
            return item;
        }

        public virtual void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _dbContext.Entry(item).State = EntityState.Modified;

        }

        // public virtual int SaveChanges()
        // {
        //     return  _dbContext.SaveChanges();
        // }

        // public virtual async System.Threading.Tasks.Task<int> SaveChangesAsync()
        // {

        //     return await _dbContext.SaveChangesAsync();
        // }

        
        public virtual bool Delete(T item)
        {
            _dbContext.Set<T>().Remove(item);
            return true;
        }

        public virtual IQueryable<T> FetchMulti(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ?  _dbContext.Set<T>() : _dbContext.Set<T>().Where(predicate);
        }

        public virtual IQueryable<T> FetchAll()
        {
            return _dbContext.Set<T>().AsNoTracking().AsQueryable();
        }
        public virtual IQueryable<T> FetchMultiAsNoTracking(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _dbContext.Set<T>().AsNoTracking() :
                 _dbContext.Set<T>().AsNoTracking().Where(predicate);
        }

        public virtual IQueryable<T> FetchMultiWithTracking(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _dbContext.Set<T>() :
                 _dbContext.Set<T>().Where(predicate);
        }

        public virtual Boolean Any(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().AsNoTracking().Any(predicate);
        }

        public virtual async Task<Boolean> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AsNoTracking().AnyAsync(predicate);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? _dbContext.Set<T>().FirstOrDefault() : _dbContext.Set<T>().FirstOrDefault(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? await _dbContext.Set<T>().FirstOrDefaultAsync() : await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public virtual T FirstOrDefaultWithReload(Expression<Func<T, bool>> predicate)
        {
            var entity = _dbContext.Set<T>().FirstOrDefault(predicate);
            if (entity == null)
                return default(T);
            _dbContext.Entry(entity).Reload();
            return entity;
        }

        public virtual async Task<T> LastOrDefaultWithReloadAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbContext.Set<T>().LastOrDefaultAsync(predicate);
            if (entity == null)
                return default(T);
            _dbContext.Entry(entity).Reload();
            return entity;
        }

        public virtual T LastOrDefault(Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? _dbContext.Set<T>().LastOrDefault() : _dbContext.Set<T>().LastOrDefault(predicate);
        }

        public virtual async Task<T> FirstOrDefaultWithReloadAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
            if (entity == null)
                return default(T);
            _dbContext.Entry(entity).Reload();
            return entity;
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().SingleOrDefault(predicate);
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? await _dbContext.Set<T>().SingleOrDefaultAsync() : await _dbContext.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public virtual int Count(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ?
            _dbContext.Set<T>().Count() :
            _dbContext.Set<T>().Count(predicate);

        }
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? await _dbContext.Set<T>().CountAsync() : await _dbContext.Set<T>().CountAsync(predicate);
        }
        public virtual int BulkUpdate(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updatePredicate)
        {
            return _dbContext.Set<T>().Where(predicate).Update(updatePredicate);
        }
        public virtual async Task<int> BulkUpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updatePredicate)
        {
            return await _dbContext.Set<T>().Where(predicate).UpdateAsync(updatePredicate);
        }

        public T FetchFirstOrDefaultAsNoTracking(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().AsNoTracking().FirstOrDefault(predicate);
        }

        public async Task<T> FetchFirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public void UpdateWithAttachUoW(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _dbContext.Set<T>().Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
        }
        public void UpdateWithAttach(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _dbContext.Set<T>().Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
        }

        public async void UpdateWithAttachAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _dbContext.Set<T>().Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
        }

        public int RunQuery(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<S> RunQuery<S>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public S RunRawQuery<S>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }




        public async Task StartTransaction(Func<Task> action)
        {

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Achieving atomicity between original Catalog database operation and the
                // IntegrationEventLog thanks to a local transaction

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    await action();
                    transaction.Commit();

                }

            });


        }
       

    }
}