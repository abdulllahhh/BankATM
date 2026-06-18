using MediatR;


namespace BuildingBlocks.SharedKernel.DomainEvents
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOnUtc { get; }
    }
}
