namespace Banking.Infrastructure.NServiceBusConfiguration
{
    public class SchemaForQueue
    {
        public SchemaForQueue()
        {
            
        }

        public string QueueName { get; set; }
        public string QueueSchema { get; set; }
    }
}