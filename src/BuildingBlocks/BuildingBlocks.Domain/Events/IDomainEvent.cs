using MediatR;

namespace BuildingBlocks.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }

    DateTime OccurredOnUtc { get; }
}