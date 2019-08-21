using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Accounts;
using MediatR;

namespace Banking.Application.Commands.Accounts
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        public WithdrawCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public async Task<bool> Handle(WithdrawCommand req, CancellationToken cancellationToken)
        {
            
            var account = await  _accountRepository.GetAsync(req.AccountId);
            
            account.Withdraw(req.Amount);


             _accountRepository.Update(account);
            await _accountRepository.UnitOfWork.SaveEntitiesAsync();
            return true;
        }
    }
}