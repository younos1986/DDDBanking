using System;
using System.Collections.Generic;
using Banking.Domain.DomainExceptions;
using Banking.Domain.Events;
using Banking.Domain.SeedWorks;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Accounts
{
    public class Account : Entity, IAggregateRoot
    {

        protected Account()
        {
            _credits = new List<Credit>();
            _debits = new List<Debit>();
        }

        public Account(long customerId, string description)
        {
            // this.Id = 1;
            this.CustomerId = customerId;
            this._accountStatusId = AccountStatus.Opened.Id;
            this.Description=description;

            _credits = new List<Credit>();
            _debits = new List<Debit>();

            this.AddDomainEvent(new AccountAddedDomainEvent(Id, CustomerId));

        }
        public long CustomerId { get; private set; }

        public string Description { get; private set; }



        public virtual AccountStatus AccountStatus {get; private set;}
        
        public AccountStatus GetAccountStatus => AccountStatus.From(_accountStatusId);

        private  int _accountStatusId;

        private readonly List<Credit> _credits;
        private readonly List<Debit> _debits;
        public virtual IReadOnlyCollection<Credit> Credits => _credits;
        public virtual IReadOnlyCollection<Debit> Debits => _debits;

        public Credit Deposit(Amount amount)
        {
            Credit c = new Credit(Id , amount, DateTime.Now , "description");
            _credits.Add(c);
            return c;
        }

        public Debit Withdraw(Amount amount)
        {
            Amount balance = GetBalance();

            if (amount > balance)
                throw new NoSufficientBalanceException($"The account {Id} doesn't have sufficient balance. Its balance is {balance}");

            Debit d = new Debit(Id, amount, DateTime.Now, "description"); ;
            _debits.Add(d);
            return d;
        }

        public void Close()
        {
            Amount balance = GetBalance();

            if (balance > 0)
                throw new AccountCannotBeClosedException($"The account {Id} cannot be closed.Current balance is {balance} ");

            //this.AccountStatus = AccountStatus.Closed;
            this._accountStatusId = AccountStatus.Closed.Id;
            this.Description = "the account is closed";
            
            this.AddDomainEvent(new AccountClosedDomainEvent(Id , CustomerId));

        }

        public void CheckIfAccountIsAllowedToGetLoan(Amount amount)
        {
            Amount balance = GetBalance();

            if (balance < 10_000)
                throw new YouAreNotAllowedTogetLoanException("");

            //todo: some other stuff
        }

        public Amount GetBalance()
        {
            Amount balance = 0;
            foreach (var c in Credits)
            {
                balance = balance + c.Amount;
            }
            
            foreach (var d in Debits)
            {
                balance = balance - d.Amount;
            }
            return balance;
        }

    }
}