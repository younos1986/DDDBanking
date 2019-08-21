using System;

namespace Banking.Domain.DomainExceptions
{
    public class DomainException : Exception
    {
        public DomainException(string businessMessage): base(businessMessage)
        {
            
        }
    }
}