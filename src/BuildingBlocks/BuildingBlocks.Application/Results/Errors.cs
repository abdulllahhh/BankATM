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
        public static Error NotFound => new("CARD.NOT_FOUND", "Card not found.");
        public static Error InvalidPin => new("CARD.INVALID_PIN", "Invalid PIN.");
        public static Error CardNotActive => new("CARD.NOT_ACTIVE", "Card is not active.");
        public static Error CardBlocked => new("CARD.BLOCKED", "Card has been blocked.");
        public static Error CardConfiscated => new("CARD.CONFISCATED", "Card has been confiscated.");
        public static Error CardExpired => new("CARD.EXPIRED", "Card has expired.");
        public static Error MaxFailedAttempts => new("CARD.MAX_FAILED_ATTEMPTS", "Maximum failed PIN attempts reached.");
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
