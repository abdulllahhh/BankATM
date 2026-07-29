using BuildingBlocks.Application.CQRS;

namespace Banking.Application.ATM.Commands.StartupATM;

public sealed record StartupATMCommand(Guid ATMId) : ICommand<StartupATMResponse>;
