using MediatR;

namespace Banking.Domain.Events
{
    public class AccountClosedDomainEvent : INotification
    {

        public long AccountId { get; private set; }
        public long CustomerId { get; private set; }
        public AccountClosedDomainEvent(long accountId, long customerId)
        {
            this.AccountId = accountId;
            this.CustomerId = customerId;


        }




    }
}