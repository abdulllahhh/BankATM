using Bank.Server.Domain.AuditContext.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Server.Domain.AuditContext.Repositories
{
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Adds a new AuditLog to the tracking state.
        /// Do NOT call SaveChanges here — the UnitOfWork pipeline handles persistence.
        /// </summary>
        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    }
}
