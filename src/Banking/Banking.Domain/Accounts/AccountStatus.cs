using System;
using System.Collections.Generic;
using System.Linq;
using Banking.Domain.DomainExceptions;
using Banking.Domain.SeedWorks;

namespace Banking.Domain.Accounts
{
    public class AccountStatus: Enumeration
    {

        public static AccountStatus Opened = new AccountStatus(1, nameof(Opened).ToLowerInvariant()); 
        public static AccountStatus  Locked = new AccountStatus(2, nameof(Locked).ToLowerInvariant());
        public static AccountStatus Closed = new AccountStatus(3, nameof(Closed).ToLowerInvariant());

        public AccountStatus(AccountStatus t) : base(t.Id, t.Name)
        {
        }
        public AccountStatus()
        {

        }
        public AccountStatus(int id, string name)
         : base(id, name)
        {
        }




        public static IEnumerable<AccountStatus> List() =>
            new[] { Opened, Locked, Closed};

        public static AccountStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static AccountStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new AccountDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}