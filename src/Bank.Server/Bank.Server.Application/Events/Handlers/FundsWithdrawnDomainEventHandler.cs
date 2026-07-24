using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AuditContext.Aggregates;
using Bank.Server.Domain.AuditContext.Repositories;
using MediatR;

namespace Bank.Server.Application.Events.Handlers;

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
    }
}
