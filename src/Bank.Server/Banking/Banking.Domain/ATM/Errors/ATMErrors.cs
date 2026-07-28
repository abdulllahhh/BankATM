namespace Banking.Domain.ATM.Errors;

public static class ATMErrors
{
    public const string NotOnline = "ATM is not online.";
    public const string NotOffline = "ATM is not offline.";
    public const string NotInMaintenance = "ATM is not in maintenance mode.";
    public const string CannotShutdown = "ATM must be online or in maintenance to shut down.";
    public const string CannotStart = "ATM must be offline to start.";
    public const string CannotStartMaintenance = "ATM must be online to start maintenance.";
    public const string CannotCompleteMaintenance = "ATM must be in maintenance mode to complete maintenance.";
    public const string CannotReplenish = "ATM must be online or in maintenance to replenish cash.";
    public const string InsufficientCash = "ATM has insufficient cash to complete this operation.";
    public const string DenominationNotFound = "The specified denomination is not available in this ATM.";
    public const string InvalidQuantity = "Quantity must be greater than zero.";
    public const string CashNotReserved = "The specified cash has not been reserved.";
}
