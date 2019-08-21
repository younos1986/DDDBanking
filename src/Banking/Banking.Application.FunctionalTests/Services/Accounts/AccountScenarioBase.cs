using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;

namespace Banking.Application.FunctionalTests.Services.Accounts
{
    public class AccountScenarioBase
    {
        public Microsoft.AspNetCore.TestHost.TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(AccountScenarioBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<Banking.Api.Startup>();

            Microsoft.AspNetCore.TestHost.TestServer testServer = null;
                try
                {
                    testServer = new TestServer(hostBuilder);
                    
                }
                catch (System.Exception ex)
                {
                    
                }
             

           

             return testServer;
         }

        public static class Get
        {
            public static string Orders = "api/v1/orders";

            public static string OrderBy(int id)
            {
                return $"api/v1/orders/{id}";
            }
        }

        public static class Post
        {
            public static string AddCustomer = "api/Customer/AddCustomer";
            public static string AddAccount = "api/Account/AddAccount";
            public static string Deposit = "api/Account/Deposit";
            public static string Withdraw = "api/Account/Withdraw";
            public static string CloseAccount = "api/Account/CloseAccount";
            public static string GetAccountStatus = "api/Account/GetAccountStatus";
        }

        public static class Put
        {
            //public static string CancelOrder = "api/v1/orders/cancel";
        }

        public static class Delete
        {
            // public static string OrderBy(int id)
            // {
            //     return $"api/v1/orders/{id}";
            // }
        }
    }
}