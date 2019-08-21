namespace Banking.Domain.DomainExceptions
{
    public class NoSufficientBalanceException: DomainException
    {
        public NoSufficientBalanceException(string businessMessage):
        base(businessMessage)
        {
            
        }
    }
}