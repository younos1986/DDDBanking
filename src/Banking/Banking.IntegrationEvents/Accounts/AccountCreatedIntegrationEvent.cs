using NServiceBus;

namespace Banking.IntegrationEvents.Accounts
{
    public class AccountCreatedIntegrationEvent : IntegrationEvent , IEvent 
    {

        public AccountCreatedIntegrationEvent(long customerId)
        {
            this.CustomerId = customerId;

        }
        public long CustomerId { get; private set; }

    }
}