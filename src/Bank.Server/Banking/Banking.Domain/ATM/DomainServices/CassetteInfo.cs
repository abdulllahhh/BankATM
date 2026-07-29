using Banking.Domain.ATM.ValueObjects;

namespace Banking.Domain.ATM.DomainServices;

public sealed record CassetteInfo(Denomination Denomination, int AvailableCount);
