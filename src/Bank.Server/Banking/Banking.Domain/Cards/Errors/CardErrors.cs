namespace Banking.Domain.Cards.Errors;

public static class CardErrors
{
    public const string CardNotActive = "Card is not active.";
    public const string CardBlocked = "Card has been blocked.";
    public const string CardConfiscated = "Card has been confiscated.";
    public const string CardExpired = "Card has expired.";
    public const string InvalidPin = "The provided PIN is incorrect.";
    public const string MaxFailedAttemptsReached = "Maximum failed PIN attempts reached. Card confiscated.";
    public const string AlreadyActive = "Card is already active.";
    public const string AlreadyBlocked = "Card is already blocked.";
    public const string AlreadyConfiscated = "Card has already been confiscated.";
}
