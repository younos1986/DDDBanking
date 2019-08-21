using System.Collections.Generic;

namespace Banking.Infrastructure.NServiceBusConfiguration
{
    public class ServiceBusConfiguration
    {
        public ServiceBusConfiguration()
        {
            
        }

        public string CuurentEndpoint { get; set; }
        public string SendFailedMessagesTo { get; set; }
        public string AuditProcessedMessagesTo { get; set; }
        public string CurrentEndpointConnectionString { get; set; }
        public string TransportConnectionString { get; set; }
        public string DefaultSchema { get; set; }
        public string PublisherEndpoint { get; set; }
        public string PublisherSenderTableName { get; set; }
        public string PublisherSchema { get; set; }
        public List<SchemaForQueue> SchemaForQueues { get; set; }
    }
}