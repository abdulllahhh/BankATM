# Aggregate Discovery

> **Version:** 1.0  
> **Last Updated:** 2026-07-25  
> **Status:** Living Document

---

## Executive Summary

This document traces the discovery, definition, and refinement of aggregate roots in the ATMSystem. Aggregates define consistency boundaries — transactional guarantees within a boundary and eventual consistency across boundaries. The final design identifies five primary aggregates: **DebitCard**, **Account**, **ATM**, **ATMTransaction**, and **ATMSession**, plus a **CashDispenser** aggregate for cash denomination management.

The discovery process balanced three forces: (1) business invariant enforcement, (2) transactional performance (avoiding overly large aggregates), and (3) the path toward future microservice extraction.

---

## Methodology

Aggregate discovery followed a four-phase process:

### Phase 1: Event Storming

Domain events were plotted on a timeline. Events that always occur together and share state were grouped as candidates for the same aggregate. Key questions asked for each group:
- "If two of these events happen simultaneously, could they conflict?"
- "Does this group have a natural lifecycle?"

### Phase 2: Business Process Analysis

Each business operation was analyzed for its transactional scope:
- **Withdraw**: touches Account (balance, daily limit), ATM (cash), Session (state), Card (attempts)
- **Validate Card**: touches Card (status, expiry), Session (state)
- **Authenticate PIN**: touches Card (PIN, attempts), Session (state)
- **Load Cash**: touches ATM (inventory only)

### Phase 3: Consistency Boundary Analysis

For each candidate group, we asked:
- **Change frequency**: How often does this data change?
- **Consistency requirements**: Does this need strong consistency or is eventual consistency acceptable?
- **Lifecycle**: Does this data have the same lifecycle as other data?

### Phase 4: Refinement Workshops

Domain experts and developers reviewed boundaries, identified hot spots, and made refinement decisions.

---

## Initial Candidate Aggregates

The initial event storming session identified these candidate groups:

| Candidate | Events Grouped | Initial Assessment |
|-----------|---------------|-------------------|
| **Card** (monolithic) | CardIssued, CardValidated, PinAuthenticated, PinAuthenticationFailed, CardConfiscated, CardBlocked, CardExpired | Too large — contained session state (start, complete) before refinement |
| **Account** | AccountCreated, FundsWithdrawn, FundsDeposited, DailyLimitExceeded | Good boundary — clear lifecycle and consistency requirements |
| **ATM** | CashDispensed, CashLoaded, ATMStarted, ATMOnline, ATMOffline | Good boundary but needed CashDispenser separation |
| **Transaction** | TransactionApproved, TransactionCompleted, TransactionCancelled | Initially combined session and transaction lifecycles |
| **Session** | (Discovered during refinement) | Initially merged into Card; separated when session state machine was identified |

---

## Refinement Decisions

### Decision 1: Separate ATMSession from DebitCard

**Problem:** The initial model merged session state (`Started`, `CardValidated`, etc.) into the `DebitCard` aggregate, assuming a card could only be in one session at a time.

**Why this was wrong:**
- A card's lifetime is **years**; a session's lifetime is **minutes**
- Session state changes every 30–60 seconds per transaction
- A card could be involved in multiple sessions across different ATMs (sequential, not concurrent)
- Merging them would cause lock contention on the card record for every ATM interaction
- Card invariants (PIN hash, status, failed attempts) are fundamentally different from session invariants (state machine sequence)

**Resolution:**
- `DebitCard` owns: PIN hash, card status, failed attempt count, expiration, issue date
- `ATMSession` owns: session state machine, current card ID (by reference), ATM ID, transaction selection, session timestamps
- Session references `CardId` as a value object, not by navigation property

### Decision 2: Separate ATMTransaction from Account

**Problem:** The initial design made `Transaction` a child entity of `Account`.

**Why this was wrong:**
- Account change frequency is moderate (balance changes per transaction). Transaction append frequency is high (every ATM interaction).
- Transaction data must be append-only for audit purposes. Account data is mutable (balance changes).
- In a future microservice extraction, Account and Transaction would be in different services.
- Querying transaction history should not lock the account.

**Resolution:**
- `Account` owns: balance, daily limit, withdrawn-today, status, currency
- `ATMTransaction` owns: transaction amount, type, currency, status, references (ATMId, AccountId, DebitCardId)
- `ATMTransaction` is append-only (status changes from Pending → Completed/Failed, but the record is never deleted)

### Decision 3: ATMSession as a New Aggregate

**Problem:** Session lifecycle was initially spread across Card (authentication) and Transaction (completion).

**Why this was wrong:**
- The session has a distinct state machine: Started → CardValidated → PinAuthenticated → TransactionSelected → Completed/Cancelled
- This lifecycle doesn't match Card's lifecycle (Issued → Active → Expired/Blocked/Confiscated)
- This lifecycle doesn't match Transaction's lifecycle (Pending → Approved → Completed/Cancelled)
- Session timeout logic (auto-cancel after inactivity) only applies to sessions, not to cards or transactions
- Failed PIN attempt tracking spans session and card boundaries, requiring coordination

**Resolution:**
- `ATMSession` was created as a new aggregate root with its own identity (`SessionId`)
- Session owns the state machine, failed PIN attempts (local tracking), and selected transaction
- Session coordinates with `DebitCard` via domain events for PIN authentication

### Decision 4: CashDispenser as a Separate Aggregate

**Decision:** `CashDispenser` was separated from `ATM` to manage cash denominations independently.

**Rationale:**
- An ATM can have multiple cash cassettes with different denominations ($5, $10, $20, $50)
- Each cassette has independent inventory (count of notes)
- Denomination management (which notes to dispense for a given amount) is a complex domain concern
- Cash loading may involve specific cassettes, not just a total amount

---

## Aggregate Definitions

### DebitCard

**Why it's an Aggregate Root:** A card is a consistency boundary for PIN verification, failed attempt tracking, and status transitions. Two simultaneous operations on the same card (e.g., PIN entry and card blocking) must be sequential.

| Aspect | Details |
|--------|---------|
| **Root ID** | `Guid` |
| **Responsibilities** | Card issuance, validation, PIN authentication, failed attempt tracking, confiscation, blocking, expiry |
| **Key Invariants** | Max 3 failed PIN attempts; card must be active for PIN auth; card must not be expired for validation; PIN is immutable after issuance |
| **Owns** | `CardNumber`, `Pin`, `ExpirationDate`, `IssueDate`, `CardStatus`, `FailedAttempts` |
| **References by ID** | `AccountId` (Guid) — the account this card is linked to |
| **Outside Boundary** | Account details (balance, daily limit), session state, ATM state |
| **Lifecycle** | `Issue()` → `Active` → `Validate()`, `AuthenticatePin()`, `Block()`, `Confiscate()`, `Expire()` |
| **Change Frequency** | Low — status changes only on events (block, confiscate, expire). Failed attempts increment per PIN entry. |

### Account

**Why it's an Aggregate Root:** Financial balance and daily limit enforcement require strong consistency. Two concurrent withdrawals must be serialized to prevent overdraft.

| Aspect | Details |
|--------|---------|
| **Root ID** | `Guid` |
| **Responsibilities** | Balance management, withdrawal validation, daily limit enforcement, currency consistency |
| **Key Invariants** | Balance must never go negative; daily limit must never be exceeded; withdrawal amount must be positive; currency must match |
| **Owns** | `AccountNumber`, `Balance` (Money), `DailyLimit` (Money), `WithdrawnToday` (Money), `AccountStatus` |
| **References by ID** | (None — accounts are identified by their own ID) |
| **Outside Boundary** | Card details, session state, transaction history, ATM state |
| **Lifecycle** | `Create()` → `Active` → `Withdraw()`, `Deposit()` → `Frozen` or `Closed` |
| **Change Frequency** | Medium — changes per transaction during business hours |

### ATM

**Why it's an Aggregate Root:** Cash inventory consistency is critical (must not dispense more cash than available). ATM status (online/offline) determines operational capability.

| Aspect | Details |
|--------|---------|
| **Root ID** | `Guid` |
| **Responsibilities** | Cash inventory management, operational status tracking, cash dispensing, cash loading |
| **Key Invariants** | Must be online for transactions; must have sufficient cash to dispense; cash inventory must never go negative |
| **Owns** | `ATMIdentifier`, `Location`, `ATMStatus`, `CashAvailable` (Money), `LastMaintenance` |
| **References by ID** | (None) |
| **Outside Boundary** | Session state, card validation, account balances, transaction history |
| **Lifecycle** | `Install()` → `Online` → `Offline` (maintenance) → `Online`; `LoadCash()`, `DispenseCash()` |
| **Change Frequency** | Low — cash changes per transaction; status changes on maintenance events |

### ATMTransaction

**Why it's an Aggregate Root:** Transaction records are append-only and serve as an audit trail. They should not be embedded in Account (too much contention) or ATM (unnecessary coupling).

| Aspect | Details |
|--------|---------|
| **Root ID** | `Guid` |
| **Responsibilities** | Recording financial transactions, tracking transaction lifecycle, providing audit history |
| **Key Invariants** | Status lifecycle must be sequential: Pending → (Approved → Completed) or Cancelled; amount must be positive |
| **Owns** | `Amount`, `Currency`, `TransactionType`, `TransactionStatus`, `Timestamp`, `FailureReason` |
| **References by ID** | `ATMId`, `AccountId`, `DebitCardId` — all by value, not navigation |
| **Outside Boundary** | Account balances, card status, ATM cash, session state |
| **Lifecycle** | `Create()` → `Pending` → `Approved` → `Completed` (or `Pending` → `Cancelled`) |
| **Change Frequency** | High — one per transaction; append-only once completed |

### ATMSession

**Why it's an Aggregate Root:** Session lifecycle has strict state machine rules. The session is short-lived but must maintain its state consistently throughout a customer interaction.

| Aspect | Details |
|--------|---------|
| **Root ID** | `SessionId` (strongly-typed) |
| **Responsibilities** | Managing session state machine, tracking failed PIN attempts, coordinating with card/account/ATM during transactions |
| **Key Invariants** | Sequential state transitions (cannot skip states); max 3 failed PIN attempts; can only complete if a transaction was selected; cannot modify after terminal state |
| **Owns** | `SessionStatus`, `ATMId`, `CardId`, `SelectedTransactionType`, `TransactionNumber`, `FailedPinAttempts`, `StartedAt`, `CompletedAt` |
| **References by ID** | `ATMId` (ATMId), `CardId` (CardId) — both value objects referencing external aggregates |
| **Outside Boundary** | Card PIN hash, account balance, ATM cash, transaction details |
| **Lifecycle** | `Start()` → `Started` → `ValidateCard()` → `CardValidated` → `Authenticate()` → `PinAuthenticated` → `SelectTransaction()` → `TransactionSelected` → `Complete()` → `Completed` (or `Cancel()` → `Cancelled` at any point) |
| **Change Frequency** | Very high — dozens of state transitions per minute per ATM; short-lived (minutes) |

### CashDispenser (Supporting Aggregate)

**Why it's an Aggregate Root:** Each cash cassette has independent inventory that must be tracked consistently.

| Aspect | Details |
|--------|---------|
| **Root ID** | `Guid` |
| **Responsibilities** | Tracking note count per denomination, enabling cash dispensing calculations |
| **Key Invariants** | Note count must never go negative |
| **Owns** | `ATMId`, `Denomination`, `Count` |
| **Outside Boundary** | ATM status, transaction details |

---

## Aggregate Boundary Decisions Explained

### Why ATMSession is Separate from DebitCard

| Aspect | ATMSession | DebitCard |
|--------|------------|-----------|
| **Lifespan** | Minutes (per ATM visit) | Years (card validity) |
| **Change Frequency** | Multiple transitions per minute | Rare (status changes on events) |
| **State Machine** | Started → Validated → Authenticated → Selected → Completed/Cancelled | Active → Blocked/Expired/Confiscated |
| **Consistency Requirement** | Sequential transitions within a single customer interaction | PIN validation, failed attempts, status changes |
| **Contention** | High — every ATM interaction would lock the card | Would become a bottleneck if merged |

**Conclusion:** Separating session from card improves concurrency, clarifies lifecycle ownership, and aligns with the real-world concept that a session is a temporary interaction with a long-lived card.

### Why ATMTransaction is Separate from Account

| Aspect | ATMTransaction | Account |
|--------|---------------|---------|
| **Mutation Pattern** | Append-only (never delete, rarely update status) | Mutable (balance, daily limit change per transaction) |
| **Query Pattern** | History queries (customer statement) | Current state queries (balance, limit) |
| **Consistency Requirement** | Eventual (transaction record can lag) | Strong (balance must be accurate immediately) |
| **Future Extraction** | Would be in a separate microservice | Would remain in Account service |
| **Growth** | Unlimited (years of transaction history) | Bounded (one record per account) |

**Conclusion:** Separating transaction records from the account aggregate prevents unbounded growth of the account aggregate and enables independent scaling of transaction history queries.

### Why ATMSession is Not Merged with ATMTransaction

| Aspect | ATMSession | ATMTransaction |
|--------|------------|----------------|
| **Scope** | One session per customer visit | Potentially multiple transactions per session |
| **State Machine** | Started → ... → Completed/Cancelled | Pending → Approved → Completed/Cancelled |
| **Lifespan** | Tied to physical ATM interaction | Persists beyond session for audit |
| **Session-level data** | Failed PIN attempts, card reference, ATM reference | Amount, account reference, type |

**Conclusion:** A session may contain zero or one transactions (a customer could cancel before selecting). The session lifecycle encompasses card validation and PIN auth that happen before any transaction is created. Keeping them separate allows each to evolve independently.

---

## Coordination Between Aggregates

Aggregates coordinate through domain events and event handlers, not direct references or cross-aggregate transactions.

### ATMSession + DebitCard Coordination (PIN Authentication)

```
1. AuthenticatePin command loads both ATMSession and DebitCard
2. DebitCard.AuthenticatePin(pin) → validates PIN, tracks attempts, may confiscate
3. ATMSession.Authenticate(isValid) → tracks session-level attempts, may cancel
4. Both are saved in same UnitOfWork (they're in the same module)
```

### Account + ATM Coordination (Withdrawal)

```
1. WithdrawCommandHandler loads Account
2. Account.Withdraw(amount, atmId) → deducts balance, raises FundsWithdrawnDomainEvent
3. SaveChanges → UnitOfWork commits
4. FundsWithdrawnDomainEvent published
5. AtmCashInventoryHandler handles event → loads ATM → ATM.DecreaseCashInventory(amount)
6. (Note: step 5 is eventually consistent — if ATM update fails, compensating action needed)
```

### ATMSession + Account Coordination (Session Completion)

```
1. Session.Complete(transactionNumber) → marks session as Completed
2. SessionCompletedDomainEvent is raised
3. Event handler records audit trail, triggers any post-session actions
```

### Coordination Boundaries

```
┌─────────────────────────────────────────────────────────┐
│              Command Transaction Boundary                │
│                                                          │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐          │
│  │ ATMSession│    │ DebitCard│    │ Account  │          │
│  │           │    │          │    │          │          │
│  │ State     │    │ PIN      │    │ Balance  │          │
│  │ Machine   │    │ Status   │    │ DailyLim │          │
│  └──────────┘    └──────────┘    └──────────┘          │
│                                                          │
│  Within a single command, multiple aggregates can be     │
│  modified atomically (same DB transaction) because       │
│  they share the same module.                             │
└─────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────┐
│              Eventual Consistency Boundary               │
│                                                          │
│  Domain Events → Event Handlers → Side effects on       │
│  other aggregates (eventually consistent)                │
│                                                          │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐          │
│  │    ATM   │    │  Tx Log  │    │Audit Log │          │
│  │ Cash     │    │ History  │    │          │          │
│  └──────────┘    └──────────┘    └──────────┘          │
└─────────────────────────────────────────────────────────┘
```

---

## Future Considerations

As the system evolves, the following aggregates may be introduced:

| Candidate Aggregate | Rationale | When to Introduce |
|--------------------|-----------|-------------------|
| **Customer** | Currently, customer data is implicit (AccountHolder string). A Customer aggregate would own customer profile, contact info, linked accounts, and authentication. | When multi-account management or customer profile features are needed |
| **CardNetwork (Visa/Mastercard)** | If the system processes inter-bank transactions, card network routing and authorization become separate concerns. | When inter-bank transactions are introduced |
| **Fee** | ATM transaction fees (surcharge, foreign ATM fee) could become complex enough for their own aggregate. | When fee structures are implemented |
| **Settlement** | End-of-day settlement between banks for inter-bank ATM transactions. | When inter-bank support is added |
| **Alert** | Customer notifications for transactions, daily limit warnings, card events. | When notification infrastructure is added |
| **Dispute** | Customer dispute tracking for unauthorized transactions. | When customer support features are added |
| **Receipt** | If receipt data becomes complex (marketing messages, QR codes), it could become its own entity. | When receipt customization is needed |
| **MaintenanceSchedule** | Scheduled maintenance tracking, technician assignment, parts inventory. | When ATM fleet management features are added |
