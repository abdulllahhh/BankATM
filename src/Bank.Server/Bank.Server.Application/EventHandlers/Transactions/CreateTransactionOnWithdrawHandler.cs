using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.DomainEvents;
using Bank.Server.Domain.TransactionContext.Aggregates;
using MediatR;

namespace Bank.Server.Application.EventHandlers.Transactions;

public sealed class CreateTransactionOnWithdrawHandler : INotificationHandler<FundsWithdrawnDomainEvent>
{
    private readonly ITransactionRepository _db;

    public CreateTransactionOnWithdrawHandler(ITransactionRepository db)
    {
        _db = db;
    }

    public async Task Handle(
        FundsWithdrawnDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var transaction = new Transaction();

        await _db.AddAsync(transaction, cancellationToken);
    }
}