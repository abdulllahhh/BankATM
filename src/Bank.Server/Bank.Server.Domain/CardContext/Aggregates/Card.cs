using Bank.Server.Domain.CardContext.ValueObjects;

namespace Bank.Server.Domain.CardContext.Aggregates
{
    public class Card
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime StartDate { get; set; }

        public CardStatus Status { get; set; }
        public string PinHash { get; set; }

        public int FailedPinAttempts { get; set; }
    }
}
