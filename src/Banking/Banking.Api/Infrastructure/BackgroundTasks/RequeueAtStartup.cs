using Banking.Domain.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Api.Infrastructure.BackgroundTasks
{
    public class RequeueAtStartup : BackgroundService
    {

        readonly BackgroundTaskConfiguration _taskConfig;
        readonly IIntegrationEventLogRepository _integrationEventLogRepository;
        public RequeueAtStartup(IConfiguration configuration
            , IIntegrationEventLogRepository integrationEventLogRepository

            )
        {
            _taskConfig = configuration.GetSection("Task_RequeueAtStartup").Get<BackgroundTaskConfiguration>();
            _integrationEventLogRepository = integrationEventLogRepository;
        }


        /// <summary>
        /// If Microservice is stopped by any exceptions or incidents, some IntegrationEvents that are in InQueue status, won't be published on next run of API
        /// this BackgroundService starts once, every time that BackgroundTasks Api starts, and it updates all InQueue statuses to ReadyToPublish again
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(new TimeSpan(0, 0, _taskConfig.Interval), stoppingToken);


                await _integrationEventLogRepository.UpdateAllInQueueToProcessToReadyToPublish();


                if (_taskConfig.IsFireAndForget == true)
                    break;
            }
            await Task.CompletedTask;
        }
    }
}
