using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AuditContext.Aggregates;
using Bank.Server.Domain.AuditContext.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Server.Application.EventHandlers.Accounts;

public sealed class FundsWithdrawnDomainEventHandler
    : INotificationHandler<FundsWithdrawnDomainEvent>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public FundsWithdrawnDomainEventHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository
            ?? throw new ArgumentNullException(nameof(auditLogRepository));
    }

    public async Task Handle(
        FundsWithdrawnDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var details = $"Amount {notification.Amount:C} withdrawn from account {notification.AccountId}. " +
                      $"Transaction reference: {notification.TransactionId}.";

        var auditLog = AuditLog.Create(
            accountId: notification.AccountId,
            actionType: "Withdrawal",
            details: details,
            correlationId: notification.TransactionId);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);

        // ❌ No SaveChanges or Commit here.
        // The AuditLog entity is now tracked by EF Core.
        // UnitOfWork.SaveChangesAsync (Phase 5) will detect HasChanges()
        // and persist it within the same open transaction.
    }
}
