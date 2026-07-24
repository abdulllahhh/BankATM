using BuildingBlocks.Domain.Common;
using System;

namespace Bank.Server.Domain.AuditContext.Aggregates
{
    public sealed class AuditLog : AggregateRoot<Guid>
    {
        private AuditLog() { }  // EF Core

        public Guid AccountId { get; private set; }

        public string ActionType { get; private set; } = string.Empty;

        public string Details { get; private set; } = string.Empty;

        public Guid CorrelationId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public static AuditLog Create(
            Guid accountId,
            string actionType,
            string details,
            Guid correlationId)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId must not be empty.", nameof(accountId));
            if (string.IsNullOrWhiteSpace(actionType))
                throw new ArgumentException("ActionType must not be empty.", nameof(actionType));
            if (string.IsNullOrWhiteSpace(details))
                throw new ArgumentException("Details must not be empty.", nameof(details));

            return new AuditLog
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                ActionType = actionType,
                Details = details,
                CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}