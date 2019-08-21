using System.Threading.Tasks;

namespace Banking.Domain.Accounts
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetAsync(long id);
    }
}