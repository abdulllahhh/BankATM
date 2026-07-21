using Bank.Server.Domain.AuditContext.Aggregates;
using Bank.Server.Domain.AuditContext.Repositories;

namespace Bank.Server.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly BankDbContext _context;

    public AuditLogRepository(BankDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        // ❌ Do NOT call SaveChanges here.
        // The UnitOfWork.SaveChangesAsync detects new ChangeTracker entries
        // and persists them inside the ambient transaction.
    }
}