namespace Banking.Domain.Cards.Errors;

public static class CardErrors
{
    public static class Validate
    {
        public const string CardNotActive = "Card is not in Active status and cannot be validated.";
        public const string CardExpired = "Card has expired.";
    }

    public static class AuthenticatePin
    {
        public const string CardNotActive = "PIN cannot be authenticated on a non-active card.";
        public const string InvalidPin = "The provided PIN is incorrect.";
        public const string CardNowConfiscated = "Maximum PIN attempts exceeded. Card has been confiscated.";
    }

    public static class IncrementFailedAttempts
    {
        public const string CardNotActive = "Failed attempts can only be incremented on an active card.";
        public const string MaxAttemptsReached = "Card is being confiscated due to maximum failed attempts.";
    }

    public static class ResetFailedAttempts
    {
        public const string CardNotActive = "Failed attempts can only be reset on an active card.";
    }

    public static class Confiscate
    {
        public const string CardNotActive = "Only an active card can be confiscated.";
        public const string AlreadyConfiscated = "Card has already been confiscated.";
    }

    public static class Block
    {
        public const string CardNotActive = "Only an active card can be blocked.";
        public const string AlreadyBlocked = "Card is already blocked.";
    }

    public static class Expire
    {
        public const string AlreadyTerminal = "Card has already been expired, blocked, or confiscated.";
    }
}
