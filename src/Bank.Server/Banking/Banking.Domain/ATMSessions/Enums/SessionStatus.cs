namespace Banking.Domain.ATMSessions.Enums;

public enum SessionStatus
{
    Started,
    CardValidated,
    PinAuthenticated,
    TransactionSelected,
    Completed,
    Cancelled
}
