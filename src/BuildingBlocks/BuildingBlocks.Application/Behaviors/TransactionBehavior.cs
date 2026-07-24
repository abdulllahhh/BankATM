using BuildingBlocks.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Application.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(
            DbContext dbContext,
            ILogger<TransactionBehavior<TRequest, TResponse>> _logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Only apply to commands (transactional requests)
            if (!IsCommandRequest())
            {
                return await next();
            }

            // Step 1: Check if there is already an active transaction
            if (_dbContext.Database.CurrentTransaction != null)
            {
                return await next();
            }

            // Step 2: Open a new execution strategy transaction (to support retries if configured)
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Step 3: Begin the transaction
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Step 4: Await the command handler execution (which calls UnitOfWork.SaveChangesAsync)
                    var response = await next();

                    // Step 5: Commit the transaction after successful execution
                    await transaction.CommitAsync(cancellationToken);

                    return response;
                }
                catch (Exception ex)
                {
                    // Step 6: Log, rollback, and rethrow the exception
                    _logger.LogError(ex, "Transaction failed for request {RequestName}. Transaction rolled back.", typeof(TRequest).Name);
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        private static bool IsCommandRequest()
        {
            var requestType = typeof(TRequest);
            return requestType.Name.EndsWith("Command") ||
                   requestType.GetInterfaces().Any(i =>
                       i == typeof(ICommand) ||
                       (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));
        }
    }
}