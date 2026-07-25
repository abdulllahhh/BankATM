using BuildingBlocks.Domain.Primitives;

namespace Banking.Domain.ATMSessions.ValueObjects;

public sealed record CardId(Guid Value) : StronglyTypedId(Value);
