using Bank.Server.Domain.AuditContext.ValueObjects;
using BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Server.Domain.AuditContext.Entities
{
    public sealed class AuditEntry : Entity<AuditEntryId>
    {
        public AuditType Type { get; }

        public Severity Severity { get; }

        public CorrelationId CorrelationId { get; }

        public string Description { get; }

        public DateTime OccurredOnUtc { get; }
    }
}
