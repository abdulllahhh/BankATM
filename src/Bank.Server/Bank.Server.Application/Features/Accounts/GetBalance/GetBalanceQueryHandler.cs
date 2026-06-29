using Bank.Server.Application.Abstractions.Persistence;
using Bank.Server.Domain.AccountContext.ValueObjects;
using MediatR;

namespace Bank.Server.Application.Features.Accounts.GetBalance
{
    public class GetBalanceQueryHandler
        : IRequestHandler<GetBalanceQuery, decimal>
    {
        private readonly IAccountRepository _repo;

        public GetBalanceQueryHandler(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<decimal> Handle(
            GetBalanceQuery request,
            CancellationToken cancellationToken)
        {
            var account = await _repo.GetByAccountNumberAsync(
                AccountNumber.Create(request.AccountNumber),
                cancellationToken);

            return account?.Balance.Amount ?? 0;
        }
    }
}