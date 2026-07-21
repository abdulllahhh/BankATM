using BuildingBlocks.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Application.Abstractions.Messaging
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(CancellationToken cancellationToken = default);
    }
}
