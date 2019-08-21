using MediatR;

namespace Banking.Application.Queries.Accounts
{
    public class AccountStatusQuery : IRequest<AccountStatusQueryResponse>
    {

        public AccountStatusQuery(long accountId)
        {
            this.AccountId = accountId;

        }
        public long AccountId { get; private set; }
    }
}