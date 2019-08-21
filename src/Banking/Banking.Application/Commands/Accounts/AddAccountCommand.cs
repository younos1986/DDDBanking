using System.Windows.Input;
using MediatR;

namespace Banking.Application.Commands
{
    public class AddAccountCommand : IRequest<bool>
    {
        public AddAccountCommand(long customerId,  string description)
        {
            this.CustomerId = customerId;
            this.Description = description;
        }
        public long CustomerId { get; private set; }
        public string Description { get; private set; }
    }
}