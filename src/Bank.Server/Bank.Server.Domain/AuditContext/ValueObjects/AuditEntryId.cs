using BuildingBlocks.Domain.Primitives;

namespace Bank.Server.Domain.AuditContext.ValueObjects;

public sealed record AuditEntryId(Guid Value) : StronglyTypedId(Value);
