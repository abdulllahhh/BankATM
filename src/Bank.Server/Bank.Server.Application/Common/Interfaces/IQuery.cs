using MediatR;

namespace Bank.Server.Application.Common.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}