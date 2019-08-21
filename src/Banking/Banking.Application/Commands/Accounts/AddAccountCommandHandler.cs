using System.Threading;
using System.Threading.Tasks;
using Banking.Application.Services;
using Banking.Domain.Accounts;
using Banking.IntegrationEvents.Accounts;
using MediatR;

namespace Banking.Application.Commands
{
    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        public AddAccountCommandHandler(
            IAccountRepository accountRepository,
        IIntegrationEventLogService integrationEventLogService)
        {
            _accountRepository= accountRepository;
            _integrationEventLogService=integrationEventLogService;
        }

        public async Task<bool> Handle(AddAccountCommand req, CancellationToken cancellationToken)
        {
            var accountCreatedIntegrationEvent = new AccountCreatedIntegrationEvent(req.CustomerId);
            await _integrationEventLogService.AddAndSaveEventAsync(accountCreatedIntegrationEvent);

            Account account = new Account(req.CustomerId , "the account is opened");

            await _accountRepository.CreateAsync(account);
            return await _accountRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}