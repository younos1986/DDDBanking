using Banking.Domain.IntegrationEvents;
using Banking.Infrastructure.NServiceBusConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Banking.Infrastructure.Extensions;

namespace Banking.Api.Infrastructure.BackgroundTasks
{
    public class EventPublisher : BackgroundService
    {


        readonly BackgroundTaskConfiguration _taskConfig;
        readonly IIntegrationEventLogRepository _integrationEventLogRepository;
        readonly IServiceBusEndpoint _endpoint;
        public EventPublisher(IConfiguration configuration
            , IIntegrationEventLogRepository integrationEventLogRepository
            , IServiceBusEndpoint nServiceBusEndpoint)
        {
            _taskConfig = configuration.GetSection("Task_EventPublisher").Get<BackgroundTaskConfiguration>();
            _integrationEventLogRepository = integrationEventLogRepository;
            _endpoint = nServiceBusEndpoint;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(new TimeSpan(0, 0, _taskConfig.Interval), stoppingToken);

                await ProccessEvents();

                if (_taskConfig.IsFireAndForget == true)
                    break;
            }
            await Task.CompletedTask;
        }

        private async Task ProccessEvents()
        {
            List<IntegrationEventLog> localIntegrationEvents = await _integrationEventLogRepository.GetAllReadyToPulishAndUpdateTheirStatuses();
            foreach (var @event in localIntegrationEvents)
            {
                object obj = @event.Content.DeserializeJson();
                var messageId = @event.MessageId;

                await _endpoint.Publish(obj, messageId).ConfigureAwait(false);

                @event.State = EventStateEnum.Published;
                await _integrationEventLogRepository.UpdateAsync(@event);
                await _integrationEventLogRepository.UnitOfWork.SaveChangesAsync();

            }
        }
    }
}
