using System.Windows.Input;
using MediatR;

namespace Banking.Application.Commands
{
    public class CloseAccountCommand : IRequest<bool>
    {

        public CloseAccountCommand(long accountId, long customerId)
        {
            this.AccountId = accountId;
            this.CustomerId = customerId;

        }
        public long AccountId { get; private set; }
        public long CustomerId { get;  set; }
    }
}