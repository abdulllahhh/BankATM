using Bank.Server.Domain.AuditContext.Aggregates;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository
{
    private readonly BankDbContext _context;

    public AuditLogRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log, CancellationToken ct)
    {
        await _context.AuditLogs.AddAsync(log, ct);
    }
}