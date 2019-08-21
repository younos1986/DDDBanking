using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Customers;
using Banking.Domain.Events;
using MediatR;

namespace Banking.Application.DomainEventHandlers.AccountClosed
{
    public class AccountClosedDomainEventHandler : INotificationHandler<AccountClosedDomainEvent>
    {

        readonly ICustomerRepository _customerRepository;
        public AccountClosedDomainEventHandler(ICustomerRepository customerRepository)
        {
            _customerRepository=customerRepository;
        }

        public async Task Handle(AccountClosedDomainEvent notification, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(q=>q.Id ==  notification.CustomerId);

            customer.SetOneAccountClosed();

            _customerRepository.Update(customer);
            
        }
    }
}