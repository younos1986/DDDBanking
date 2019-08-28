using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banking.Domain.IntegrationEvents;
using Banking.Domain.SeedWorks;
using Banking.Infrastructure.DbContexts;
using Banking.Infrastructure.Extensions;
using Banking.Infrastructure.NServiceBusConfiguration;
using Banking.Infrastructure.Repositories;
using Banking.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NServiceBus;

namespace Banking.Application.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {

        private readonly List<Type> _eventTypes;
        readonly BankingDbContext _bankingDbContext;
        private readonly IServiceBusEndpoint _endpoint;
        public IntegrationEventLogService(
            BankingDbContext bankingDbContext
            ,IServiceBusEndpoint endpoint
            )
        {
            _bankingDbContext=bankingDbContext;
            _endpoint = endpoint;
        }

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            return await _bankingDbContext.IntegrationEventLogs
                .Where(e => e.TransactionId == tid && e.State == EventStateEnum.ReadyToPublish)
                .OrderBy(o => o.CreationTime)
                //.Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)))
                .ToListAsync();
        }

        private Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLog(@event, transaction.TransactionId);
            //if log DbContext is different from the main, pass the current tranmsaction to its DbContext
            //_bankingDbContext.Database.UseTransaction(transaction.GetDbTransaction());
            _bankingDbContext.IntegrationEventLogs.Add(eventLogEntry);
            return _bankingDbContext.SaveChangesAsync();
        }
        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _bankingDbContext.IntegrationEventLogs.Single(ie => ie.MessageId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _bankingDbContext.IntegrationEventLogs.Update(eventLogEntry);

            return _bankingDbContext.SaveChangesAsync();
        }



        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
               // _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {

                    await MarkEventAsInProgressAsync(logEvt.MessageId);

                    var deserializedObject = logEvt.Content.DeserializeJson();
                    await _endpoint.Publish(deserializedObject, logEvt.MessageId);

                    await MarkEventAsPublishedAsync(logEvt.MessageId);

                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await MarkEventAsFailedAsync(logEvt.MessageId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            //_logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await SaveEventAsync(evt, _bankingDbContext.GetCurrentTransaction());
        }


    }
}