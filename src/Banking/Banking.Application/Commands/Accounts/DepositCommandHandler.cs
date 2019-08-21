using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Accounts;
using MediatR;

namespace Banking.Application.Commands.Accounts
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        public DepositCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public async Task<bool> Handle(DepositCommand req, CancellationToken cancellationToken)
        {
            
            var account = await  _accountRepository.GetAsync(req.AccountId);
            
            account.Deposit(req.Amount);


             _accountRepository.Update(account);
            await _accountRepository.UnitOfWork.SaveEntitiesAsync();
            return true;
        }
    }
}