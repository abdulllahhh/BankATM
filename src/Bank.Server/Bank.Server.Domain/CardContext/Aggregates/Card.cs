using Bank.Server.Domain.CardContext.DomainEvents;
using Bank.Server.Domain.CardContext.ValueObjects;
namespace Bank.Server.Domain.CardContext.Aggregates
{

    public sealed class Card
        : AggregateRoot<Guid>
    {
        private Card()
        {
        }

        public CardNumber CardNumber { get; private set; }

        public string PinHash { get; private set; }

        public CardStatus Status { get; private set; }

        public int FailedPinAttempts { get; private set; }

        public DateOnly StartDate { get; private set; }

        public DateOnly ExpiryDate { get; private set; }

        public static Card Issue(
            CardNumber cardNumber,
            string pinHash,
            DateOnly startDate,
            DateOnly expiryDate)
        {
            var card = new Card
            {
                Id = Guid.NewGuid(),
                CardNumber = cardNumber,
                PinHash = pinHash,
                Status = CardStatus.Active,
                FailedPinAttempts = 0,
                StartDate = startDate,
                ExpiryDate = expiryDate
            };

            card.RaiseDomainEvent(
                new CardIssuedDomainEvent(
                    card.Id));

            return card;
        }
    }
}