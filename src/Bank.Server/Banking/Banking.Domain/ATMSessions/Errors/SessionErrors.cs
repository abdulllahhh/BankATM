namespace Banking.Domain.ATMSessions.Errors;

public static class SessionErrors
{
    public static class Start
    {
        public const string SessionAlreadyExists = "A session with this ID already exists.";
    }

    public static class ValidateCard
    {
        public const string InvalidState = "Card can only be validated when the session is in 'Started' state.";
    }

    public static class Authenticate
    {
        public const string InvalidState = "PIN can only be authenticated when the session is in 'CardValidated' state.";
        public const string InvalidPin = "The provided PIN is incorrect.";
        public const string MaxAttemptsExceeded = "Maximum PIN attempts exceeded. Session has been cancelled.";
    }

    public static class SelectTransaction
    {
        public const string InvalidState = "Transaction can only be selected when the session is in 'PinAuthenticated' state.";
    }

    public static class Complete
    {
        public const string InvalidState = "Session can only be completed when a transaction has been selected.";
        public const string AlreadyTerminated = "Session has already been completed or cancelled.";
    }

    public static class Cancel
    {
        public const string AlreadyTerminated = "Session has already been completed or cancelled.";
    }

    public static class EjectCard
    {
        public const string SessionNotTerminated = "Card can only be ejected after the session has been completed or cancelled.";
    }
}
