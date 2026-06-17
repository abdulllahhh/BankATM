using Bank.Server.Application.Commands;
using Bank.Server.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Server.Api.Endpoints
{
    /// <summary>
    /// Handles transaction operations including cash withdrawals, balance queries, and fund transfers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly WithdrawCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController"/> class.
        /// </summary>
        /// <param name="handler">The handler for processing withdrawal commands.</param>
        public TransactionsController(WithdrawCommandHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Performs a cash withdrawal from the specified account.
        /// </summary>
        /// <param name="command">The withdrawal details, including card number, account number, and amount.</param>
        /// <returns>A status indicating the success of the operation.</returns>
        /// <response code="200">Withdrawal completed successfully.</response>
        /// <response code="400">Invalid withdrawal request or insufficient funds.</response>
        [HttpPost("withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Withdraw(WithdrawCommand command)
        {
            await _handler.Handle(command);
            return Ok();
        }

        /// <summary>
        /// Queries the current balance of the specified account.
        /// </summary>
        /// <param name="cardNumber">The ATM card number associated with the account.</param>
        /// <param name="accountNumber">The account number to query.</param>
        /// <returns>The current balance of the account.</returns>
        /// <response code="200">Returns the account balance details.</response>
        /// <response code="400">Invalid account or card details.</response>
        [HttpGet("balance")]
        [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetBalance([FromQuery] string cardNumber, [FromQuery] string accountNumber)
        {
            var response = new BalanceResponse
            {
                AccountNumber = accountNumber,
                Balance = 1250.75m
            };
            return Ok(response);
        }

        /// <summary>
        /// Performs a fund transfer from one account to another.
        /// </summary>
        /// <param name="command">The transfer details, including source account, destination account, and amount.</param>
        /// <returns>A status indicating the success of the transfer.</returns>
        /// <response code="200">Transfer completed successfully.</response>
        /// <response code="400">Insufficient funds or invalid accounts.</response>
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Transfer([FromBody] TransferCommand command)
        {
            return Ok();
        }
    }

    /// <summary>
    /// Request model for performing a bank transfer.
    /// </summary>
    public class TransferCommand
    {
        /// <summary>
        /// The sender's card number.
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// The source account number.
        /// </summary>
        public string SourceAccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// The destination account number.
        /// </summary>
        public string DestinationAccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// The transfer amount.
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Response model containing the current account balance.
    /// </summary>
    public class BalanceResponse
    {
        /// <summary>
        /// The account number queried.
        /// </summary>
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// The current balance.
        /// </summary>
        public decimal Balance { get; set; }
    }
}

