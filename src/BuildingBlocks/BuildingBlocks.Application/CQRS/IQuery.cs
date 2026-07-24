using BuildingBlocks.Application.Results;
using MediatR;

namespace BuildingBlocks.Application.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
