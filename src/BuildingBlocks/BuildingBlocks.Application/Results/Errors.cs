namespace BuildingBlocks.Application.Results;

public static class Errors
{
    public static class General
    {
        public static Error Unexpected => new("GENERAL.UNEXPECTED", "An unexpected error occurred.");
    }

    public static class Account
    {
        public static Error NotFound => new("ACCOUNT.NOT_FOUND", "Account not found.");
        public static Error InsufficientFunds => new("ACCOUNT.INSUFFICIENT_FUNDS", "Insufficient funds.");
    }

    public static class Card
    {
        public static Error InvalidPin => new("CARD.INVALID_PIN", "Invalid PIN.");
    }

    public static class ATM
    {
        public static Error CashUnavailable => new("ATM.CASH_UNAVAILABLE", "Cash is unavailable.");
    }

    public static class Transaction
    {
        public static Error DailyLimitExceeded => new("TRANSACTION.DAILY_LIMIT_EXCEEDED", "Daily limit exceeded.");
    }
}
