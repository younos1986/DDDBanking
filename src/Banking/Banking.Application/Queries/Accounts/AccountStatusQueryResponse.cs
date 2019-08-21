namespace Banking.Application.Queries.Accounts
{
    public class AccountStatusQueryResponse 
    {


        public AccountStatusQueryResponse(int accountStatusId, string accountStatusDescription)
        {
            this.AccountStatusId = accountStatusId;
            this.AccountStatusDescription = accountStatusDescription;

        }
        public int AccountStatusId { get; private set; }
        public string AccountStatusDescription { get; private set; }

    }
}