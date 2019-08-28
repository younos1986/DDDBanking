using System;
using Newtonsoft.Json;

namespace Banking.IntegrationEvents
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            MessageId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid messageId, DateTime createDate)
        {
            MessageId = messageId;
            CreationDate = createDate;
        }

        [JsonProperty]
        public Guid MessageId { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }
    }
}