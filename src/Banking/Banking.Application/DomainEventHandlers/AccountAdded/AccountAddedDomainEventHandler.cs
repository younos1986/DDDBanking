using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Customers;
using Banking.Domain.Events;
using MediatR;

namespace Banking.Application.DomainEventHandlers.AccountClosed
{
    public class AccountAddedDomainEventHandler : INotificationHandler<AccountAddedDomainEvent>
    {

        readonly ICustomerRepository _customerRepository;
        public AccountAddedDomainEventHandler(ICustomerRepository customerRepository)
        {
            _customerRepository=customerRepository;
        }

        public async Task Handle(AccountAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(q=>q.Id ==  notification.CustomerId);

            customer.SetOneAccountAdded();

             _customerRepository.Update(customer);
            
        }
    }
}