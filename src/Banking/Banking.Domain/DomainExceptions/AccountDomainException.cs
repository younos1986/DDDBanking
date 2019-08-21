namespace Banking.Domain.DomainExceptions
{
    public class AccountDomainException : DomainException
    {
        public AccountDomainException(string businessMessage) : base(businessMessage)
        {
        }
    }
}