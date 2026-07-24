using MediatR;

namespace BuildingBlocks.Application;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}