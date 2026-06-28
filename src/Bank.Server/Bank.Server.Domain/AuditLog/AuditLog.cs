namespace Bank.Server.Domain.AuditLog;

public class AuditLog
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = default!;

    public string Data { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
}