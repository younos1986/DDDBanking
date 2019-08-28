using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.IntegrationEvents
{
    public interface IIntegrationEventLogRepository : IRepository<IntegrationEventLog>
    {
        Task<List<IntegrationEventLog>> GetAllReadyToPulishAndUpdateTheirStatuses();

        Task UpdateAllInQueueToProcessToReadyToPublish();
        Task UpdateAsync(IntegrationEventLog model);
    }

}
