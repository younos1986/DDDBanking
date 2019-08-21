using System;
using System.Threading;
using System.Threading.Tasks;
using Banking.Domain.Customers;
using MediatR;

namespace Banking.Application.Commands.Customers
{
    public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, long>
    {

        readonly ICustomerRepository _customerRepository;
        public AddCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository=customerRepository;
        }
        public async Task<long> Handle(AddCustomerCommand req, CancellationToken cancellationToken)
        {

            var newCustomer = new Customer(req.FirstName , req.LastName , DateTime.Now.AddYears(-31));

            // newCustomer.SetAddress();

            await _customerRepository.CreateAsync(newCustomer);
            await _customerRepository.UnitOfWork.SaveEntitiesAsync();

            return newCustomer.Id;
        }
    }
}