using System;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Accounts
{
    public class Debit : Entity
    {
        protected Debit()
        {

        }
        public Debit(long accountId, Amount amount, DateTime createdAt, string description)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.CreatedAt = createdAt;
            this.Description = description;

        }
        public long AccountId { get; private set; }
        public Amount Amount { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string Description { get; private set; }
        public virtual Account Account { get; set; }


    }
}