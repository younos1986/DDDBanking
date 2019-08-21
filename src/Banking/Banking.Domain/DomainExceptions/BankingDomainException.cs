using System;

namespace Banking.Domain.DomainExceptions
{
    public class BankingDomainException : Exception
    {
        public BankingDomainException()
        { }

        public BankingDomainException(string message)
            : base(message)
        { }

        public BankingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}