namespace Banking.Domain.DomainExceptions
{
    public class YouAreNotAllowedTogetLoanException: DomainException
    {
        public YouAreNotAllowedTogetLoanException(string businessMessage):base(businessMessage)
        {
            
        }
        
    }
}