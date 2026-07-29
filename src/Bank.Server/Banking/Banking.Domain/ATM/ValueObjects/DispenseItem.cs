namespace Banking.Domain.ATM.ValueObjects;

public sealed record DispenseItem(Denomination Denomination, int Quantity);
