using System;
using System.Threading.Tasks;

namespace Banking.Infrastructure.NServiceBusConfiguration
{
    public interface IServiceBusEndpoint
    {
        Task<Guid> Publish(object message, Guid? messageId);
        Task<string> Publish(object message);
        Task<string> Send(object message);
    }
}