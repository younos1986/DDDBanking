using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Banking.Domain.SeedWorks;
using Banking.IntegrationEvents;
using Newtonsoft.Json;

namespace Banking.Domain.IntegrationEvents
{
    public class IntegrationEventLog
    {
        public IntegrationEventLog() { }
        public IntegrationEventLog(IntegrationEvent @event, Guid transactionId)
        {
            MessageId = @event.MessageId;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.None;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }

        //NServiceBus Message Id
        public Guid MessageId { get; private set; }
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
        public string TransactionId { get; private set; }

        

        public object DeserializeJsonContent(Type type)
        {
            return JsonConvert.DeserializeObject(Content, type);
        }
    }
}
