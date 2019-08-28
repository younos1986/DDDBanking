using Banking.Domain.IntegrationEvents;
using Banking.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Repositories
{
    public class IntegrationEventLogRepository : Repository<IntegrationEventLog>, IIntegrationEventLogRepository
    {
        public IntegrationEventLogRepository(BankingDbContext dbContext) : base(dbContext)
        {
        }



        public async Task<List<IntegrationEventLog>> GetAllReadyToPulishAndUpdateTheirStatuses()
        {

            var query = await FetchMulti(q => q.State == EventStateEnum.ReadyToPublish)
                .OrderBy(q => q.MessageId)
                .Take(50)
                .ToListAsync();

            var selectedIds = query.Select(q => q.MessageId);

            if (!selectedIds.Any())
                return new List<IntegrationEventLog>();

            await BulkUpdateAsync(q => selectedIds.Contains(q.MessageId),
                q => new IntegrationEventLog()
                {
                    State = EventStateEnum.InProgress
                });

            return query;
        }
        
        public async Task UpdateAllInQueueToProcessToReadyToPublish()
        {
            await BulkUpdateAsync(q => q.State == EventStateEnum.InProgress,
                q => new IntegrationEventLog()
                {
                    State = EventStateEnum.ReadyToPublish
                });
        }

        public async Task UpdateAsync(IntegrationEventLog model)
        {
            await UpdateAsync(model);
        }
    }
}
