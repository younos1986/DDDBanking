using System;
using System.Threading;
using System.Threading.Tasks;
using Banking.Application.Services;
using Banking.Infrastructure.DbContexts;
using Banking.Infrastructure.Extensions;
using Banking.Infrastructure.NServiceBusConfiguration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace Banking.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly BankingDbContext _dbContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        private readonly IServiceBusEndpoint _endpoint;
        public TransactionBehaviour(BankingDbContext dbContext,
            IIntegrationEventLogService integrationEventLogService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger
            ,IServiceBusEndpoint endpoint
            )
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(BankingDbContext));
            //_orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentException(nameof(orderingIntegrationEventService));
            _integrationEventLogService = integrationEventLogService;
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));

            _endpoint = endpoint;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {

                //return await next();

                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    //using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    await _integrationEventLogService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}