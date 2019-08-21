using MediatR;

namespace Banking.Application.Commands.Accounts
{
    public class DepositCommand : IRequest<bool>
    {

        public DepositCommand(long accountId, decimal amount)
        {
            this.AccountId = accountId;
            this.Amount = amount;

        }
        public long AccountId { get; private set; }
        public decimal Amount { get; private set; }

    }
}