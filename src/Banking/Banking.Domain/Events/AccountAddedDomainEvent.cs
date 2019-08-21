using MediatR;

namespace Banking.Domain.Events
{
    public class AccountAddedDomainEvent: INotification
    {
        public long AccountId { get; private set; }
        public long CustomerId { get; private set; }
        public AccountAddedDomainEvent(long accountId, long customerId)
        {
            this.AccountId = accountId;
            this.CustomerId = customerId;


        }
    }
}