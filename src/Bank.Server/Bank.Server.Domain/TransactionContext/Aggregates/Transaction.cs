using Bank.Server.Domain.TransactionContext.ValueObjects;


namespace Bank.Server.Domain.TransactionContext.Aggregates
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }

        public decimal Amount { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }

        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
