using System;
using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Accounts;
using MediatR;

namespace Banking.Application.Queries.Accounts
{
    public class AccountStatusQueryHandler : IRequestHandler<AccountStatusQuery, AccountStatusQueryResponse>
    {

        readonly IAccountRepository _accountRepository;
        public  AccountStatusQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository=accountRepository;
        }


        public async Task<AccountStatusQueryResponse> Handle(AccountStatusQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetAsync(request.AccountId);

            if (account == null)
                throw new Exception($"AccountId {request.AccountId}  not found");

            var response = new AccountStatusQueryResponse(account.AccountStatus.Id , account.AccountStatus.Name);
            return response;

        }
    }
}