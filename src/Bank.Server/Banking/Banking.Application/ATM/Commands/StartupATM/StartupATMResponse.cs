using Banking.Domain.ATM.Enums;

namespace Banking.Application.ATM.Commands.StartupATM;

public sealed record StartupATMResponse(
    Guid ATMId,
    ATMStatus Status);
