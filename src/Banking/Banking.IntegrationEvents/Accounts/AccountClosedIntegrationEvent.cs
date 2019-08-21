using NServiceBus;

namespace Banking.IntegrationEvents.Accounts
{
    public class AccountClosedIntegrationEvent : IntegrationEvent, IEvent 
    {

        public AccountClosedIntegrationEvent(long customerId , long accountId)
        {
            this.CustomerId = customerId;
            this.AccountId = accountId;
        }

        
        public long CustomerId { get; private set; }
        public long AccountId { get; private set; }

    }
}