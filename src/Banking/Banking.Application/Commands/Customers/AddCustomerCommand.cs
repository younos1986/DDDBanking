using MediatR;

namespace Banking.Application.Commands.Customers
{
    public class AddCustomerCommand : IRequest<long>
    {

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public AddCustomerCommand(string firstName,string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;

        }
        

    }
}