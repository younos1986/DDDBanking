using System.Collections.Generic;
using System;
namespace Banking.Domain.Customers
{
    public class Customer : Entity, IAggregateRoot
    {

        
        public Customer(string firstName, string lastName, DateTime birthDay)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.BirthDay = birthDay;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime BirthDay { get; private set; }
        
        public int AccountNumber { get; private set; }

        
        //public IList<long> Accounts { get; private set; }

        public void SetOneAccountClosed()
        {
            AccountNumber = AccountNumber-1;
        }

        public void SetOneAccountAdded()
        {
            AccountNumber = AccountNumber + 1;
        }


    }
}