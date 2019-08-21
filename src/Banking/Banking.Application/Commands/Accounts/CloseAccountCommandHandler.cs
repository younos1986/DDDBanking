using System.Threading;
using System.Threading.Tasks;
using Banking.Application.Services;
using Banking.Domain.Accounts;
using Banking.IntegrationEvents.Accounts;
using MediatR;

namespace Banking.Application.Commands
{
    public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, bool>
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        public CloseAccountCommandHandler(
            IAccountRepository accountRepository ,
            IIntegrationEventLogService integrationEventLogService)
        {
            _accountRepository= accountRepository;
            _integrationEventLogService = integrationEventLogService;
        }


        public async Task<bool> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            //var accountClosedIntegrationEvent = new AccountClosedIntegrationEvent(request.CustomerId, request.AccountId);
            //await _integrationEventLogService.AddAndSaveEventAsync(accountClosedIntegrationEvent);

            var account = await _accountRepository.GetAsync(request.AccountId);

            account.Close();

             _accountRepository.Update(account);
            await _accountRepository.UnitOfWork.SaveEntitiesAsync();
            return true;
        }
    }
}