using Banking.Domain.Customers;
using Banking.Infrastructure.DbContexts;

namespace Banking.Infrastructure.Repositories
{
    public class CustomerRepository: Repository<Customer> , ICustomerRepository
    {
        public CustomerRepository(BankingDbContext _dbContext):base(_dbContext)
        {

        }
    }
}