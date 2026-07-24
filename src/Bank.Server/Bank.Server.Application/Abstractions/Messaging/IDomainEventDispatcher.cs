namespace Bank.Server.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken);
}