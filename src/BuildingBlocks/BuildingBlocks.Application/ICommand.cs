using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Application;

public interface ICommand : IRequest<Result>
{
}
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}