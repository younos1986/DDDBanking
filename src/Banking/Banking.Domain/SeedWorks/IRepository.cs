using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Banking.Domain.SeedWorks;

namespace Banking.Domain
{
    public interface IRepository<T>  : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> CreateAsync(T item);
        void Update(T item);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> FetchMulti(Expression<Func<T, bool>> predicate = null);
    }
}