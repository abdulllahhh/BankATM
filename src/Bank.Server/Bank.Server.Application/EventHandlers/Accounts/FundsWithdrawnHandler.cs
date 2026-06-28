using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AuditLog;
using MediatR;
using System.Text.Json;

public sealed class FundsWithdrawnHandler : INotificationHandler<FundsWithdrawnDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public FundsWithdrawnHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        FundsWithdrawnDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var audit = new AuditLog
        {
            Id = Guid.NewGuid(),
            EventType = nameof(FundsWithdrawnDomainEvent),
            Data = JsonSerializer.Serialize(notification),
            CreatedAtUtc = DateTime.UtcNow
        };

        // ❗ Still missing DbSet access → FIX BELOW
    }
}