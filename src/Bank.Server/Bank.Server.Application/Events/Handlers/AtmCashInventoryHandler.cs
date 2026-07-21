using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AccountContext.ValueObjects;
using MediatR;

namespace Bank.Server.Application.Events.Handlers;

public sealed class AtmCashInventoryHandler
    : INotificationHandler<FundsWithdrawnDomainEvent>
{
    private readonly IATMRepository _atmRepository;

    public AtmCashInventoryHandler(IATMRepository atmRepository)
    {
        _atmRepository = atmRepository;
    }

    public async Task Handle(
        FundsWithdrawnDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var atm = await _atmRepository.GetByIdAsync(
            notification.AtmId,
            cancellationToken);

        if (atm is null)
        {
            throw new InvalidOperationException(
                $"ATM '{notification.AtmId}' was not found.");
        }

        var result = atm.DecreaseCashInventory(
            Money.Create(notification.Amount, notification.Currency));

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }
    }
}
