using System.Threading.Tasks;
using Banking.Domain.Accounts;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        BankingDbContext myOwnDbContext;
        public AccountRepository(BankingDbContext dbContext) : base(dbContext)
        {
            myOwnDbContext=dbContext;
        }


        public async Task<Account> GetAsync(long id) 
        {
            
            var entity = await _dbContext.Set<Account>().FindAsync(id);
            if (entity != null)
            {
                    await _dbContext.Entry(entity)
                        .Collection(q=>q.Credits).LoadAsync();

                    await _dbContext.Entry(entity)
                    .Collection(q=>q.Debits).LoadAsync();

                    await _dbContext.Entry(entity)
                        .Reference(q=>q.AccountStatus).LoadAsync();
            }

            return entity;
        }



    }
}