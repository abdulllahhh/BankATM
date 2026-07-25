using Banking.Domain.ATMSessions.Enums;
using Banking.Domain.ATMSessions.Errors;
using Banking.Domain.ATMSessions.Events;
using Banking.Domain.ATMSessions.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.ATMSessions.Aggregate;

public sealed class ATMSession : AggregateRoot<SessionId>
{
    private const int MaxFailedPinAttempts = 3;

    public ATMId ATMId { get; private set; } = null!;
    public CardId? CardId { get; private set; }
    public SessionStatus Status { get; private set; }
    public TransactionType? SelectedTransactionType { get; private set; }
    public TransactionNumber? TransactionNumber { get; private set; }
    public int FailedPinAttempts { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ATMSession() { }

    private ATMSession(SessionId id, ATMId atmId) : base(id)
    {
        ATMId = atmId;
        Status = SessionStatus.Started;
        StartedAt = DateTime.UtcNow;
    }

    public static ATMSession Start(SessionId id, ATMId atmId)
    {
        var session = new ATMSession(id, atmId);
        session.RaiseDomainEvent(new SessionStartedDomainEvent(id, atmId, session.StartedAt));
        return session;
    }

    public void ValidateCard(CardId cardId)
    {
        Guard.CheckRule(new SessionMustBeInStatusRule(Status, SessionStatus.Started, Errors.SessionErrors.ValidateCard.InvalidState));

        CardId = cardId;
        Status = SessionStatus.CardValidated;
        RaiseDomainEvent(new CardValidatedDomainEvent(Id, cardId, DateTime.UtcNow));
    }

    public void Authenticate(bool isPinValid)
    {
        Guard.CheckRule(new SessionMustBeInStatusRule(Status, SessionStatus.CardValidated, Errors.SessionErrors.Authenticate.InvalidState));

        if (isPinValid)
        {
            Status = SessionStatus.PinAuthenticated;
            RaiseDomainEvent(new PinAuthenticatedDomainEvent(Id, DateTime.UtcNow));
        }
        else
        {
            FailedPinAttempts++;

            if (FailedPinAttempts >= MaxFailedPinAttempts)
            {
                Status = SessionStatus.Cancelled;
                CompletedAt = DateTime.UtcNow;
                RaiseDomainEvent(new SessionCancelledDomainEvent(Id, Errors.SessionErrors.Authenticate.MaxAttemptsExceeded, CompletedAt.Value));
            }
        }
    }

    public void SelectTransaction(TransactionType transactionType)
    {
        Guard.CheckRule(new SessionMustBeInStatusRule(Status, SessionStatus.PinAuthenticated, Errors.SessionErrors.SelectTransaction.InvalidState));

        SelectedTransactionType = transactionType;
        Status = SessionStatus.TransactionSelected;
    }

    public void Complete(TransactionNumber transactionNumber)
    {
        Guard.CheckRule(new SessionMustNotBeTerminalRule(Status, Errors.SessionErrors.Complete.AlreadyTerminated));
        Guard.CheckRule(new SessionMustBeInStatusRule(Status, SessionStatus.TransactionSelected, Errors.SessionErrors.Complete.InvalidState));

        TransactionNumber = transactionNumber;
        Status = SessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new SessionCompletedDomainEvent(Id, transactionNumber, CompletedAt.Value));
    }

    public void Cancel(string? reason = null)
    {
        Guard.CheckRule(new SessionMustNotBeTerminalRule(Status, Errors.SessionErrors.Cancel.AlreadyTerminated));

        Status = SessionStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new SessionCancelledDomainEvent(Id, reason ?? "Session cancelled by user.", CompletedAt.Value));
    }

    public void EjectCard()
    {
        Guard.CheckRule(new SessionMustBeTerminalRule(Status, Errors.SessionErrors.EjectCard.SessionNotTerminated));
    }

    private sealed class SessionMustBeInStatusRule : IBusinessRule
    {
        private readonly SessionStatus _currentStatus;
        private readonly SessionStatus _expectedStatus;
        private readonly string _message;

        public SessionMustBeInStatusRule(SessionStatus currentStatus, SessionStatus expectedStatus, string message)
        {
            _currentStatus = currentStatus;
            _expectedStatus = expectedStatus;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() => _currentStatus != _expectedStatus;
    }

    private sealed class SessionMustNotBeTerminalRule : IBusinessRule
    {
        private readonly SessionStatus _status;
        private readonly string _message;

        public SessionMustNotBeTerminalRule(SessionStatus status, string message)
        {
            _status = status;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() => _status is SessionStatus.Completed or SessionStatus.Cancelled;
    }

    private sealed class SessionMustBeTerminalRule : IBusinessRule
    {
        private readonly SessionStatus _status;
        private readonly string _message;

        public SessionMustBeTerminalRule(SessionStatus status, string message)
        {
            _status = status;
            _message = message;
        }

        public string Message => _message;

        public bool IsBroken() => _status is not SessionStatus.Completed and not SessionStatus.Cancelled;
    }
}
