namespace Banking.Domain.DomainExceptions
{
    public class AccountCannotBeClosedException : DomainException
    {
        public AccountCannotBeClosedException(string businessMessage):
        base(businessMessage)
        {
            
        }
    }
}