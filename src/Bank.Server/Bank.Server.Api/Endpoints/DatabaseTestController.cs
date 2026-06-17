using Bank.Server.Domain.AccountContext.Aggregates;
using Bank.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank.Server.Api.Endpoints
{
    /// <summary>
    /// Test endpoint to verify create and read operations against the PostgreSQL database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseTestController : ControllerBase
    {
        private readonly BankDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTestController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public DatabaseTestController(BankDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Seeds a test account into the database.
        /// </summary>
        /// <returns>The created test account details.</returns>
        [HttpPost("seed-test-account")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> SeedTestAccount()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = $"ACC-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                Balance = 1000.00m,
                DailyLimit = 500.00m,
                WithdrawnToday = 0.00m
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }

        /// <summary>
        /// Retrieves all accounts from the database.
        /// </summary>
        /// <returns>A list of all accounts.</returns>
        [HttpGet("accounts")]
        [ProducesResponseType(typeof(IEnumerable<Account>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }
    }
}
