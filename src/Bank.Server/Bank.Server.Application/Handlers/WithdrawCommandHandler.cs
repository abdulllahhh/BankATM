using Bank.Server.Application.Commands;
using Bank.Server.Domain.Entities;
using Bank.Server.Domain.ValueObjects;


namespace Bank.Server.Application.Handlers
{
    public class WithdrawCommandHandler
    {
        //private readonly IAccountRepository _accounts;
        //private readonly ITransactionRepository _transactions;

        public async Task Handle(WithdrawCommand command)
        {
            //var account = await _accounts.GetByNumber(command.AccountNumber);

            //if (account == null)
            //    throw new Exception("Account not found");

            //if (account.Balance < command.Amount)
            //    throw new Exception("Insufficient funds");

            //account.Balance -= command.Amount;
            //account.WithdrawnToday += command.Amount;

            //var transaction = new Transaction
            //{
            //    Id = Guid.NewGuid(),
            //    Type = TransactionType.Withdraw,
            //    Amount = command.Amount,
            //    Status = TransactionStatus.Completed
            //};

            //await _transactions.Add(transaction);
            //await _accounts.Update(account);
        }
    }
}
