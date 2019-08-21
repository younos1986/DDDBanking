using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Banking.Infrastructure.NServiceBusConfiguration.SqlHelpers;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Transport.SQLServer;

namespace Banking.Infrastructure.NServiceBusConfiguration
{
    public class ServiceBusEndpoint: IDisposable, IServiceBusEndpoint
    {
        // more help can be foound here
        //https://docs.particular.net/samples/endpoint-configuration/   
        
        
            private static IEndpointInstance endpointInstance;

            readonly ServiceBusConfiguration config;
            public ServiceBusEndpoint(IConfiguration _configuration)
            {
                config = _configuration.GetSection("NServiceBusConfiguration").Get<ServiceBusConfiguration>();
                Initialize().Wait();
            }


            private async Task Initialize()
            {
                if (endpointInstance != null)
                    return;

                if (config == null)
                    throw new Exception("NServiceBusConfiguration section in appsetting.json file is not provided");


                var senderDb = config.CurrentEndpointConnectionString; // "Data Source=.;Initial Catalog=Sender;Persist Security Info=True;User ID=sa;Password=AAaa123;Max Pool Size=80";
                var transportDb = config.TransportConnectionString; // "Data Source=.;Initial Catalog=SqlTransport;Persist Security Info=True;User ID=sa;Password=AAaa123;Max Pool Size=80";

                #region SqlServer Transport
                Console.Title = "Ticket.Sender";
                var endpointConfiguration = new EndpointConfiguration(config.CuurentEndpoint);
                endpointConfiguration.SendFailedMessagesTo(config.SendFailedMessagesTo);


                ////Configuring endpoints for monitoring _ SendHeartbeat
                //endpointConfiguration.SendHeartbeatTo(
                //    serviceControlQueue: "Particular.ServiceControl",
                //    frequency: TimeSpan.FromSeconds(15),
                //    timeToLive: TimeSpan.FromSeconds(30));


                //endpointConfiguration.ReportCustomChecksTo(
                //    serviceControlQueue: "Particular.ServiceControl",
                //    timeToLive: TimeSpan.FromSeconds(10));


                // Turn on auditing.
                endpointConfiguration.AuditProcessedMessagesTo(config.AuditProcessedMessagesTo);

                //endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();

                #region SenderConfiguration

                var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
                transport.ConnectionString(transportDb);
                transport.DefaultSchema(config.DefaultSchema);
                transport.UseSchemaForQueue(config.SendFailedMessagesTo, "dbo");
                transport.UseSchemaForQueue(config.AuditProcessedMessagesTo, "dbo");
                transport.UseNativeDelayedDelivery().DisableTimeoutManagerCompatibility();



                //endpointConfiguration.UsePersistence<InMemoryPersistence>();
                var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
                dialect.Schema(config.DefaultSchema);
                persistence.ConnectionBuilder(
                    connectionBuilder: () =>
                    {
                        return new SqlConnection(senderDb);
                    });
                persistence.TablePrefix("");
                var subscriptions = persistence.SubscriptionSettings();
                subscriptions.CacheFor(TimeSpan.FromMinutes(1));

                // Use JSON.NET serializer
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                //Enable the Outbox
                endpointConfiguration.EnableOutbox();

                // Make sure NServiceBus creates queues in RabbitMQ, tables in SQL Server, etc.
                // You might want to turn this off in production, so that DevOps can use scripts to create these.
                endpointConfiguration.EnableInstallers();



                // Define conventions
                //var conventions = endpointConfiguration.Conventions();
                //conventions.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("IntegrationEvent"));

                #endregion
                SqlHelper.CreateSchema(senderDb, config.DefaultSchema);
                SqlHelper.CreateSchema(transportDb, config.DefaultSchema);
                endpointInstance = await Endpoint.Start(endpointConfiguration)
                    .ConfigureAwait(false);
                #endregion




                #region RabbitMQ Transport Configuration 
                //var endpointConfiguration = new EndpointConfiguration(config.CuurentEndpoint);

                //// Configure RabbitMQ transport
                //var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                //transport.UseConventionalRoutingTopology();
                //transport.ConnectionString(GetRabbitConnectionString());

                //// Configure persistence
                //var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
                //persistence.SqlDialect<SqlDialect.MsSqlServer>();
                //persistence.ConnectionBuilder(connectionBuilder:
                //    () => new SqlConnection(config.ConnectionString));

                //// Use JSON.NET serializer
                //endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                //// Enable the Outbox
                //endpointConfiguration.EnableOutbox();

                //// Make sure NServiceBus creates queues in RabbitMQ, tables in SQL Server, etc.
                //// You might want to turn this off in production, so that DevOps can use scripts to create these.
                //endpointConfiguration.EnableInstallers();

                //// Turn on auditing.
                //endpointConfiguration.AuditProcessedMessagesTo("audit");

                //// Define conventions
                ////var conventions = endpointConfiguration.Conventions();
                ////conventions.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("IntegrationEvent"));

                //// Configure the DI container.
                ////endpointConfiguration.UseContainer<AutofacBuilder>(customizations: customizations =>
                ////{
                ////    customizations.ExistingLifetimeScope(container);
                ////});

                //// Start the endpoint and register it with ASP.NET Core DI
                //endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult(); 

                //private string GetRabbitConnectionString()
                //{
                //    return $"host=127.0.0.1";
                //}            

                #endregion

            }


            public async Task<Guid> Publish(object message, Guid? messageId)
            {
                if (messageId == null)
                    messageId = Guid.NewGuid();

                var options = new PublishOptions();
                options.SetMessageId(messageId.ToString());

                await endpointInstance.Publish(message, options)
                    .ConfigureAwait(false);

                return messageId.Value;
            }


            /// <summary>
            /// customize endpointInstance.Publish to send a custome UniqueId to be able to save the UniqueId in LocalIntegrationEvent
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async Task<string> Publish(object message)
            {
                var messageId = Guid.NewGuid();
                var options = new PublishOptions();
                options.SetMessageId(messageId.ToString());

                await endpointInstance.Publish(message, options)
                    .ConfigureAwait(false);

                return messageId.ToString();
            }

            public async Task<string> Send(object message)
            {
                var messageId = Guid.NewGuid();
                var options = new SendOptions();
                options.SetMessageId(messageId.ToString());

                await endpointInstance.Send(message, options)
                    .ConfigureAwait(false);

                return messageId.ToString();
            }


            public void Dispose()
            {
                DisposeResources().Wait();

            }
            private async Task DisposeResources()
            {
                await endpointInstance.Stop().ConfigureAwait(false);
            }

        }
    }
