using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.AccountContext.ValueObjects;
using Bank.Server.Domain.TransactionContext.Aggregates;
using MediatR;

namespace Bank.Server.Application.Events.Handlers;

public sealed class BankingTransactionHandler
    : INotificationHandler<FundsWithdrawnDomainEvent>
{
    private readonly ITransactionRepository _transactionRepository;

    public BankingTransactionHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(
        FundsWithdrawnDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var transaction = Transaction.CreateWithdrawal(
            notification.TransactionId,
            notification.AccountId,
            Money.Create(notification.Amount, notification.Currency));

        await _transactionRepository.AddAsync(
            transaction,
            cancellationToken);
    }
}
