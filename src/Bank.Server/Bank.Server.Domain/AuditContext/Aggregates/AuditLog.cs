using Bank.Server.Domain.AuditContext.Entities;
using BuildingBlocks.Domain;

namespace Bank.Server.Domain.AuditContext.Aggregates
{
    public sealed class AuditLog : AggregateRoot<AuditLogId>
    {
        private readonly List<AuditEntry> _entries = new();

        public IReadOnlyCollection<AuditEntry> Entries => _entries;

        public void AddEntry(
            AuditEntry entry)
        {
            _entries.Add(entry);
        }
    }

}