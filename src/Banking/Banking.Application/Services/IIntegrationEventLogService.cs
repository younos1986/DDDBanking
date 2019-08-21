using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Banking.Domain.IntegrationEvents;
using Banking.Domain.SeedWorks;
using Banking.IntegrationEvents;
using Microsoft.EntityFrameworkCore.Storage;

namespace Banking.Application.Services
{
    public interface IIntegrationEventLogService
    {

        Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        // Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);

        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
         
    }
}