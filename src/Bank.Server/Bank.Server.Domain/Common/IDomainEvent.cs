namespace Bank.Server.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}