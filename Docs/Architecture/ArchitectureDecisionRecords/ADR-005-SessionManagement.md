# ADR-005

**Title:** Session Management Aggregate Design

**Status:** Accepted

**Date:** 2026-07-25

## Context

The ATM system requires an interactive session flow that spans multiple steps:

1. Customer inserts a debit card into the ATM
2. Card is validated (active, not expired)
3. Customer enters their PIN and is authenticated
4. Customer selects a transaction type (withdrawal, deposit, balance inquiry)
5. Transaction is completed or session is cancelled (customer cancels, PIN attempts exceeded, timeout)

Prior to this ADR, session logic was scattered across the ATM context and the application layer with no dedicated aggregate. There was no explicit state machine to enforce the **sequential ordering** of steps — a customer could theoretically request a transaction before validating their card or authenticating their PIN.

The existing domain model had no concept of a session; the `ATM` aggregate tracked operational status but not per-customer interaction state.

## Decision

We will create a **dedicated `ATMSession` aggregate** in its own modular sub-folder under `Banking.Domain.ATMSessions/`.

### Key Design Elements

#### 1. Strongly-Typed IDs

All identifiers referenced by the session are strongly‑typed value objects to prevent primitive obsession and type confusion:

| Value Object | Underlying Type | Purpose |
|---|---|---|
| `SessionId` | `Guid` | Uniquely identifies the session |
| `ATMId` | `Guid` | Identifies the ATM hosting the session |
| `CardId` | `Guid` | Identifies the inserted debit card |
| `TransactionNumber` | `string` (generated `TXN-{ts}-{random}`) | Identifies the completed transaction |

`SessionId`, `ATMId`, and `CardId` extend `StronglyTypedId` from `BuildingBlocks.Domain.Primitives`. `TransactionNumber` extends `ValueObject` with a `Generate()` factory method.

#### 2. State Machine with `SessionStatus`

The session lifecycle is modeled as an explicit state machine using a `SessionStatus` enum:

```
Started ──► CardValidated ──► PinAuthenticated ──► TransactionSelected ──► Completed
                                                                              │
                              ◄────── Cancelled ◄─────────────────────────────┘
```

Allowed transitions:

| Current Status | Method | Next Status |
|---|---|---|
| *(new)* | `ATMSession.Start()` | `Started` |
| `Started` | `ValidateCard(CardId)` | `CardValidated` |
| `CardValidated` | `Authenticate(true)` | `PinAuthenticated` |
| `CardValidated` | `Authenticate(false)` (attempts < 3) | `CardValidated` (increments `FailedPinAttempts`) |
| `CardValidated` | `Authenticate(false)` (attempts >= 3) | `Cancelled` |
| `PinAuthenticated` | `SelectTransaction(TransactionType)` | `TransactionSelected` |
| `TransactionSelected` | `Complete(TransactionNumber)` | `Completed` |
| Any non‑terminal | `Cancel(reason)` | `Cancelled` |
| `Completed` / `Cancelled` | `EjectCard()` | *(no state change; validates terminal status)* |

#### 3. Static Factory Method

```csharp
public static ATMSession Start(SessionId id, ATMId atmId)
{
    var session = new ATMSession(id, atmId);
    session.RaiseDomainEvent(new SessionStartedDomainEvent(id, atmId, session.StartedAt));
    return session;
}
```

The constructor is private. All creation flows through `Start()`, which immediately raises `SessionStartedDomainEvent`.

#### 4. Instance Methods for State Transitions

Each transition is an instance method that:

- Invokes `Guard.CheckRule()` with a private `IBusinessRule` implementation to enforce the current status invariant
- Mutates the aggregate state
- Raises the corresponding domain event

```csharp
public void ValidateCard(CardId cardId)
{
    Guard.CheckRule(new SessionMustBeInStatusRule(
        Status, SessionStatus.Started, SessionErrors.ValidateCard.InvalidState));
    CardId = cardId;
    Status = SessionStatus.CardValidated;
    RaiseDomainEvent(new CardValidatedDomainEvent(Id, cardId, DateTime.UtcNow));
}
```

#### 5. Business Rule Enforcement via `Guard.CheckRule`

Two private business rule implementations enforce invariants:

| Rule Class | Purpose |
|---|---|
| `SessionMustBeInStatusRule` | Ensures the session is in the exact expected state before a transition |
| `SessionMustNotBeTerminalRule` | Prevents operations (Complete, Cancel) on already‑terminated sessions |
| `SessionMustBeTerminalRule` | Ensures the session is terminal before card ejection |

#### 6. Domain Events Raised per Transition

| Method | Event Raised | Payload |
|---|---|---|
| `Start()` | `SessionStartedDomainEvent` | `SessionId`, `ATMId`, `StartedAt` |
| `ValidateCard()` | `CardValidatedDomainEvent` | `SessionId`, `CardId`, `ValidatedAt` |
| `Authenticate(true)` | `PinAuthenticatedDomainEvent` | `SessionId`, `AuthenticatedAt` |
| `Complete()` | `SessionCompletedDomainEvent` | `SessionId`, `TransactionNumber`, `CompletedAt` |
| `Cancel()` | `SessionCancelledDomainEvent` | `SessionId`, `Reason`, `CancelledAt` |

Note: `Authenticate(false)` does **not** raise a `PinAuthenticatedDomainEvent`. Failed PIN attempts are tracked locally. Only when max attempts are exceeded does the session cancel and raise `SessionCancelledDomainEvent`.

#### 7. Module Organization

Per ADR-006, the ATMSession aggregate is organized in a feature‑aligned module:

```
Banking.Domain/
  ATMSessions/
    Aggregate/
      ATMSession.cs
    ValueObjects/
      SessionId.cs
      ATMId.cs
      CardId.cs
      TransactionNumber.cs
    Events/
      SessionStartedDomainEvent.cs
      CardValidatedDomainEvent.cs
      PinAuthenticatedDomainEvent.cs
      SessionCompletedDomainEvent.cs
      SessionCancelledDomainEvent.cs
    Enums/
      SessionStatus.cs
      TransactionType.cs
    Errors/
      SessionErrors.cs
```

#### 8. EF Core Mapping

The `ATMSession` aggregate is persisted via EF Core with value object conversions. Strongly‑typed IDs are mapped to their underlying `Guid` values. EF Core configuration is handled in `ATMSessionConfiguration`.

## Consequences

### Positive

- **Clear lifecycle:** The state machine makes the valid sequence of operations explicit and impossible to violate.
- **Testability:** Each transition can be unit‑tested in isolation. The aggregate has no infrastructure dependencies.
- **Event‑driven audit:** Every state transition raises a domain event, enabling the Audit context to build a complete trace of every session.
- **Extraction‑ready:** The aggregate is self‑contained within its own module namespace. It can be extracted to a microservice with minimal changes.
- **Business rule isolation:** Invariant logic is encapsulated in private nested `IBusinessRule` classes within the aggregate, following the existing `DebitCard` pattern.

### Negative

- **More classes:** The module introduces 15+ new files (aggregate, 4 value objects, 5 events, 2 enums, 1 errors class).
- **EF Core complexity:** Strongly‑typed IDs and value objects require custom value converter mappings in EF Core configuration.
- **Cross‑context references:** `CardId` and `ATMId` are value objects that duplicate the identity types from other aggregates. This is intentional (loose coupling) but adds some mapping overhead.

### Mitigations

- Value object mappings are centralized in `ATMSessionConfiguration` using EF Core's `OwnsOne()` and `HasConversion()` patterns.
- `TransactionNumber` uses a deterministic generation format (`TXN-{timestamp}-{random}`) to support idempotency and debugging.

## Compliance

- This ADR is consistent with **ADR-001 (DDD)** — the session is a distinct aggregate with clear boundaries and ubiquitous language.
- This ADR is consistent with **ADR-002 (Clean Architecture)** — the aggregate lives in the Domain layer with zero infrastructure dependencies.
- This ADR is consistent with **ADR-004 (Modular Monolith)** — the aggregate is organized as a module within the monolith, with explicit namespace boundaries for future extraction.
- This ADR is consistent with **ADR-006 (Modular Domain Organization)** — the aggregate follows the feature‑aligned module structure (`Aggregate/`, `ValueObjects/`, `Events/`, `Enums/`, `Errors/`).

## References

- ADR-001: Domain Driven Design
- ADR-004: Modular Monolith
- ADR-006: Modular Domain Organization
- `Banking.Domain.ATMSessions.Aggregate.ATMSession` — Implementation
- `Banking.Domain.ATMSessions.Enums.SessionStatus` — State enum
- `Banking.Domain.ATMSessions.ValueObjects.SessionId` — Strongly‑typed ID
- `Banking.Domain.Aggregates.ATM` — Existing ATM aggregate
