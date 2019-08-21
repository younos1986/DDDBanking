using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Banking.Application.Commands;
using Banking.Application.Commands.Customers;
using Banking.Application.FunctionalTests.Extensions;
using Banking.Application.Queries.Accounts;
using Newtonsoft.Json;
using Xunit;

namespace Banking.Application.FunctionalTests.Services.Accounts
{
    public class AccountScenario : AccountScenarioBase
    {
        [Fact]
        public async Task  Creat_Account_Deposit_Withdraw_Close_Account()
        {
            using (var accountServer = new AccountScenarioBase().CreateServer())
            {

                var accountClient = accountServer.CreateIdempotentClient(new Uri("https://localhost:5000"));


                // create a customer
                var customerData = new StringContent(BuildCustomer(), UTF8Encoding.UTF8, "application/json");
                var addCustomerResponse = await accountClient.PostAsync(AccountScenarioBase.Post.AddCustomer, customerData);


                addCustomerResponse.EnsureSuccessStatusCode();
                var addCustomerString = await addCustomerResponse.Content.ReadAsStringAsync();
                var customerId = JsonConvert.DeserializeObject<long>(addCustomerString);

                // create an account
                var accountData = new StringContent(BuildAccount(customerId), UTF8Encoding.UTF8, "application/json");
                var addAccountResponse = await accountClient.PostAsync(AccountScenarioBase.Post.AddAccount, accountData);

                addAccountResponse.EnsureSuccessStatusCode();
                var addAccountString = await addAccountResponse.Content.ReadAsStringAsync();
                var accountId = JsonConvert.DeserializeObject<long>(addAccountString);

                // Deposit an account
                var depositData = new StringContent(BuildDeposit(accountId), UTF8Encoding.UTF8, "application/json");
                 await accountClient.PostAsync(AccountScenarioBase.Post.Deposit, depositData);

                // Withdraw an account
                var withdrawData = new StringContent(BuildWithdraw(accountId), UTF8Encoding.UTF8, "application/json");
                await accountClient.PostAsync(AccountScenarioBase.Post.Withdraw, withdrawData);

                // Close the account
                var closeData = new StringContent(BuildClose(accountId , customerId), UTF8Encoding.UTF8, "application/json");
                await accountClient.PostAsync(AccountScenarioBase.Post.CloseAccount, closeData);

                // check the account
                var checkCloseData = new StringContent(BuildCheckCloseData(accountId), UTF8Encoding.UTF8, "application/json");
                var checkCloseDataResponse = await accountClient.PostAsync(AccountScenarioBase.Post.GetAccountStatus, checkCloseData);
                checkCloseDataResponse.EnsureSuccessStatusCode();
                var checkCloseDataString = await checkCloseDataResponse.Content.ReadAsStringAsync();
                var getAccountStatusQueryResponse = JsonConvert.DeserializeObject<AccountStatusQueryResponse>(checkCloseDataString);
                //get account and verify the account is closed

                // Assert.Equal(getAccountStatusQueryResponse.AccountStatusId , 3);
                Assert.Equal(getAccountStatusQueryResponse.AccountStatusDescription, "Closed");

            }

        }


        string BuildCustomer()
        {
            var command = new AddCustomerCommand("Younes" , "Baghaie Moghaddam");
            return JsonConvert.SerializeObject(command);
        }
        string BuildAccount(long customerId)
        {
            var command = new AddAccountCommand(customerId ,  "account is opened");
            return JsonConvert.SerializeObject(command);
        }

        string BuildDeposit(long accountId)
        {
            var command = new Banking.Application.Commands.Accounts.DepositCommand(accountId , 100);
            return JsonConvert.SerializeObject(command);
        }

        string BuildWithdraw(long accountId)
        {
            var command = new Banking.Application.Commands.Accounts.WithdrawCommand(accountId, 100);
            return JsonConvert.SerializeObject(command);
        }

        string BuildClose(long accountId , long customerId)
        {
            var command = new CloseAccountCommand(accountId,  customerId);
            return JsonConvert.SerializeObject(command);
        }

        string BuildCheckCloseData(long accountId)
        {
            var command = new AccountStatusQuery(accountId);
            return JsonConvert.SerializeObject(command);
        }

        
        
    }
}