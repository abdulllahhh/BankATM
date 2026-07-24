using BuildingBlocks.Application.Results;
using MediatR;

namespace BuildingBlocks.Application.CQRS;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
