using BuildingBlocks.Domain.Primitives;

namespace Banking.Domain.ATMSessions.ValueObjects;

public sealed record SessionId(Guid Value) : StronglyTypedId(Value);
