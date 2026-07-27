# Event Storming

> **Version:** 1.0  
> **Last Updated:** 2026-07-25  
> **Status:** Living Document

---

## Executive Summary

Event Storming is a collaborative workshop technique used to discover the domain model through the lens of domain events — things that happened in the system. This document captures the results of Event Storming sessions conducted for the ATMSystem, mapping out all flows (happy path, failure modes, maintenance operations) as sequences of domain events, commands, and aggregate interactions.

The process revealed five aggregate boundaries — DebitCard, Account, ATM, ATMSession, and ATMTransaction — by identifying where consistency requirements diverge. It also surfaced hot spots around cross-aggregate coordination and eventual consistency requirements.

---

## Methodology

The Event Storming sessions followed the standard "Big Picture" to "Process Level" to "Design Level" progression:

1. **Chaotic Exploration** — Domain experts and developers posted all domain events on a timeline without concern for ordering.
2. **Enforce Timeline** — Events were reorganized chronologically to identify flows.
3. **Add Commands and Actors** — Commands (arrows pointing into the system) and actors (who triggers what) were added.
4. **Add Aggregates** — Events were grouped by aggregate boundaries based on consistency needs.
5. **Identify Hot Spots** — Areas of uncertainty, policy gaps, and technical constraints were flagged.

---

## Event Storming Legend

```
┌──────────────────────┐
│     Domain Event     │  Orange sticky — something that happened in the domain
│  (Past Tense Verb)   │  e.g., "Card Validated", "Funds Withdrawn"
└──────────────────────┘

┌──────────────────────┐
│       Command        │  Blue sticky — an action/intent that triggers events
│   (Imperative Verb)  │  e.g., "Authenticate PIN", "Complete Withdrawal"
└──────────────────────┘

┌──────────────────────┐
│       Aggregate      │  Large yellow sticky — consistency boundary
│  (Noun, Capitalized) │  e.g., "ATMSession", "Account", "DebitCard"
└──────────────────────┘

┌──────────────────────┐
│        Actor         │  Small pink sticky — who performs the action
│    (User/Role/System)│  e.g., "Customer", "System", "Bank Operator"
└──────────────────────┘

┌──────────────────────┐
│        Policy        │  Purple sticky — business rules / constraints
│   (If/Then/When)     │  e.g., "If 3 failed PIN attempts → Confiscate Card"
└──────────────────────┘

┌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌┐
╎      Hot Spot         ╎  Red sticky — question, risk, or uncertainty
╎   (Question/Block)    ╎  e.g., "What happens if ATM goes offline mid-session?"
└╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌┘

┌──────────────────────┐
│      Read Model      │  Green sticky — data displayed to the user
│    (Information)      │  e.g., "Balance", "Transaction History"
└──────────────────────┘
```

---

## The Happy Path

The primary customer flow — card insertion through successful withdrawal and session completion.

```mermaid
sequenceDiagram
    participant Customer as Customer
    participant ATM as ATM Terminal
    participant Session as ATMSession Aggregate
    participant Card as DebitCard Aggregate
    participant Account as Account Aggregate
    participant ATMDev as ATM Aggregate
    participant Tx as ATMTransaction Aggregate

    Customer->>ATM: Insert Card
    ATM->>Session: StartSession(ATMId)
    Session-->>Session: SessionStarted

    ATM->>Card: ValidateCard(CardNumber)
    Card-->>Card: CardValidated
    ATM->>Session: ValidateCard(CardId)
    Session-->>Session: CardValidated

    Customer->>ATM: Enter PIN
    ATM->>Card: AuthenticatePin(PIN)
    Card-->>Card: PinAuthenticated
    ATM->>Session: Authenticate(isValid=true)
    Session-->>Session: PinAuthenticated

    Customer->>ATM: Select Withdrawal
    ATM->>Session: SelectTransaction(Withdrawal)
    Session-->>Session: TransactionSelected

    Customer->>ATM: Enter Amount
    ATM->>Tx: CreateTransaction(amount)
    Tx-->>Tx: TransactionCreated
    ATM->>Account: Withdraw(amount)
    Account-->>Account: FundsWithdrawn
    ATM->>ATMDev: DispenseCash(amount)
    ATMDev-->>ATMDev: CashDispensed
    ATM->>Session: Complete(TxNumber)
    Session-->>Session: SessionCompleted

    ATM->>Customer: Dispense Cash
    ATM->>Customer: Eject Card
    Customer->>Customer: Take Cash & Card
```

```mermaid
flowchart LR
    subgraph Events[Domain Events Raised]
        E1[SessionStarted]
        E2[CardValidated]
        E3[PinAuthenticated]
        E4[FundsWithdrawn]
        E5[CashDispensed]
        E6[SessionCompleted]
    end

    subgraph Aggregates[Aggregates Involved]
        A1[ATMSession]
        A2[DebitCard]
        A3[Account]
        A4[ATM]
        A5[ATMTransaction]
    end

    A1 --> E1
    A2 --> E2
    A2 --> E3
    A3 --> E4
    A4 --> E5
    A1 --> E6
```

---

## Failed Authentication Path

When a customer enters an incorrect PIN, the system tracks failed attempts and confiscates the card after 3 failures.

```mermaid
flowchart TD
    Start([Customer Inserts Card]) --> Validate[System Validates Card]
    Validate --> Prompt[System Prompts for PIN]
    Prompt --> Enter[Customer Enters PIN]

    Enter --> Check{Is PIN Valid?}

    Check -->|Yes| Success[PIN Authenticated - Proceed]
    Check -->|No| Fail[PinAuthenticationFailed]
    Fail --> Track[Increment Failed Attempts]
    Track --> CheckMax{Attempts >= 3?}

    CheckMax -->|No| RetryPrompt[Prompt for PIN Again]
    RetryPrompt --> Enter

    CheckMax -->|Yes| Confiscate[CardConfiscated]
    Confiscate --> SessionCancel[SessionCancelled - Max Attempts]
    SessionCancel --> End([Card Retained by ATM])

    subgraph Events
        Fail
        Confiscate
        SessionCancel
    end

    subgraph Policies
        P1[Policy: Max 3 PIN Attempts]
        P2[Policy: Confiscate on 3rd Failure]
    end
```

### Retry Scenario

```mermaid
sequenceDiagram
    participant Customer as Customer
    participant ATM as ATM Terminal
    participant Session as ATMSession
    participant Card as DebitCard

    Customer->>ATM: Enter Wrong PIN
    ATM->>Session: Authenticate(isValid=false)
    Session-->>Session: FailedPinAttempts=1
    Card-->>Card: PinAuthenticationFailed (1 attempt)
    ATM->>Customer: "Incorrect PIN. 2 attempts remaining."

    Customer->>ATM: Enter Wrong PIN Again
    ATM->>Session: Authenticate(isValid=false)
    Session-->>Session: FailedPinAttempts=2
    Card-->>Card: PinAuthenticationFailed (2 attempts)
    ATM->>Customer: "Incorrect PIN. 1 attempt remaining."

    Customer->>ATM: Enter Wrong PIN Again
    ATM->>Session: Authenticate(isValid=false)
    Session-->>Session: FailedPinAttempts=3
    Session-->>Session: SessionCancelled (MaxAttemptsExceeded)
    Card-->>Card: CardConfiscated
    ATM->>Customer: "Card retained. Contact your bank."
```

---

## Withdrawal Failures

Multiple failure modes exist during the withdrawal flow, each raising specific events and providing customer feedback.

```mermaid
flowchart TD
    Enter([Customer Selects Withdrawal]) --> Amount[Customer Enters Amount]
    Amount --> Validate{Validate Amount}

    Validate -->|Not Positive| Error1[Error: Invalid Amount]
    Error1 --> Prompt[Prompt Again]

    Validate -->|Valid Amount| CheckSession{Session in<br/>TransactionSelected?}
    CheckSession -->|No| Error2[Error: Session State Invalid]
    CheckSession -->|Yes| CheckATM{ATM Online?}

    CheckATM -->|No| Error3[Error: ATM Offline]
    CheckATM -->|Yes| CheckCash{ATM Has<br/>Sufficient Cash?}

    CheckCash -->|No| Error4[Error: Insufficient ATM Cash]
    CheckCash -->|Yes| CheckAccount{Account Active?}

    CheckAccount -->|No| Error5[Error: Account Not Active]
    CheckAccount -->|Yes| CheckCurrency{Currency<br/>Matches?}

    CheckCurrency -->|No| Error6[Error: Currency Mismatch]
    CheckCurrency -->|Yes| CheckFunds{Sufficient<br/>Funds?}

    CheckFunds -->|No| Error7[Error: Insufficient Funds]
    CheckFunds -->|Yes| CheckLimit{Within<br/>Daily Limit?}

    CheckLimit -->|No| Error8[DailyLimitExceeded]
    CheckLimit -->|Yes| Success([Withdrawal Processed])

    Error1 --> Prompt
    Error2 --> CancelSession[Cancel Session]
    Error3 --> CancelSession
    Error4 --> Notify[Notify Customer]
    Notify --> Retry{Retry?}
    Retry -->|Yes| Amount
    Retry -->|No| CancelSession
    Error5 --> CancelSession
    Error6 --> CancelSession
    Error7 --> Notify
    Error8 --> Notify
```

```mermaid
sequenceDiagram
    participant Cust as Customer
    participant ATM as ATM Terminal
    participant Session as ATMSession
    participant Acct as Account
    participant ATMM as ATM Machine

    Cust->>ATM: Request $500 Withdrawal
    ATM->>Session: SelectTransaction(Withdrawal)
    Session-->>Session: TransactionSelected
    ATM->>Acct: Withdraw($500)

    alt Insufficient Funds
        Acct-->>Acct: DailyLimitExceeded (or Insufficient Funds)
        Acct-->>ATM: Result.Failure("Insufficient funds")
        ATM->>Session: Cancel("Insufficient funds")
        Session-->>Session: SessionCancelled
        ATM->>Cust: "Insufficient funds"
    else ATM No Cash
        Acct-->>Acct: FundsWithdrawn
        ATM->>ATMM: DispenseCash($500)
        ATMM-->>ATM: Result.Failure("Insufficient ATM cash")
        ATM->>Acct: Reverse withdrawal (compensating action)
        ATM->>Session: Cancel("ATM out of cash")
        Session-->>Session: SessionCancelled
        ATM->>Cust: "ATM unavailable. Try again."
    else Success
        Acct-->>Acct: FundsWithdrawn
        ATM->>ATMM: DispenseCash($500)
        ATMM-->>ATMM: CashDispensed
        ATM->>Session: Complete(TxNumber)
        Session-->>Session: SessionCompleted
        ATM->>Cust: Dispense $500
    end
```

---

## Transfer Failures

Transfers involve two accounts (source and destination) and must validate both.

```mermaid
flowchart TD
    Start([Customer Selects Transfer]) --> Enter[Enter Destination Account & Amount]
    Enter --> Validate{Validate Input}

    Validate -->|Invalid Account| Err1[Error: Invalid Account Number]
    Validate -->|Invalid Amount| Err2[Error: Invalid Amount]
    Validate -->|Valid| CheckSource{Source<br/>Account Active?}

    CheckSource -->|No| Err3[Error: Source Account Inactive]
    CheckSource -->|Yes| CheckDest{Destination<br/>Account Active?}

    CheckDest -->|No| Err4[Error: Destination Account Inactive]
    CheckDest -->|Yes| CheckCurrency{Currencies<br/>Match?}

    CheckCurrency -->|No| Err5[Error: Currency Mismatch]
    CheckCurrency -->|Yes| CheckFunds{Sufficient<br/>Funds?}

    CheckFunds -->|No| Err6[Error: Insufficient Funds]
    CheckFunds -->|Yes| CheckLimit{Within<br/>Daily Limit?}

    CheckLimit -->|No| Err7[DailyLimitExceeded]
    CheckLimit -->|Yes| Success([Transfer Processed])

    subgraph Events on Success
        W[FundsWithdrawn - Source]
        D[FundsDeposited - Destination]
        T[TransactionCompleted]
    end
```

```mermaid
sequenceDiagram
    participant Cust as Customer
    participant ATM as ATM Terminal
    participant Src as Source Account
    participant Dest as Destination Account

    Cust->>ATM: Transfer $200 to Account-456
    ATM->>Src: Validate Active + Currency
    ATM->>Dest: Validate Active + Currency

    alt Source Invalid
        Src-->>ATM: Account not active
        ATM->>Cust: "Source account unavailable"
    else Destination Invalid
        Dest-->>ATM: Account not found
        ATM->>Cust: "Destination account not found"
    else Insufficient Funds
        Src-->>Src: DailyLimitExceeded
        Src-->>ATM: "Daily limit exceeded"
        ATM->>Cust: "Daily withdrawal limit exceeded"
    else Success
        Src-->>Src: FundsWithdrawn
        Dest-->>Dest: FundsDeposited
        ATM->>Cust: "Transfer successful"
    end
```

---

## ATM Maintenance Events

ATM maintenance operations managed by bank operators and technicians.

```mermaid
flowchart TD
    Start([ATM Online]) --> Event1{Maintenance<br/>Required?}

    Event1 -->|Yes| GoOffline[ATM Set to Offline]
    GoOffline --> NotifyBank[Notification to Bank Operator]
    NotifyBank --> TechDispatch[Technician Dispatched]

    TechDispatch --> Maintenance{Maintenance Type}
    Maintenance -->|Cash Load| LoadCash[CashLoaded Event]
    Maintenance -->|Repair| Repair[Physical Repair]
    Maintenance -->|Software| Update[Software Update]

    LoadCash --> VerifyCash[Verify Cash Inventory]
    Repair --> TestATM[Test ATM Functions]
    Update --> Restart[Restart ATM]

    VerifyCash --> GoOnline[ATM Set to Online]
    TestATM --> GoOnline
    Restart --> GoOnline

    GoOnline --> End([ATM Ready for Customers])

    subgraph Events
        GoOffline
        LoadCash
        GoOnline
    end
```

```mermaid
sequenceDiagram
    participant Op as Bank Operator
    participant ATM as ATM Machine
    participant Tech as Technician

    Op->>ATM: Take Offline (Maintenance)
    ATM-->>ATM: Status = Offline
    Op->>Tech: Dispatch for cash loading

    Tech->>ATM: Load Cash Cassettes
    ATM-->>ATM: CashLoaded ($10,000)
    Tech->>Op: Confirm cash loaded

    Op->>ATM: Set Online
    ATM-->>ATM: Status = Online
    ATM-->>ATM: Ready for transactions
```

### Cash Inventory Lifecycle

```mermaid
stateDiagram-v2
    [*] --> Online: ATM Startup
    Online --> Offline: Maintenance Required
    Online --> Offline: System Error
    Offline --> CashLoading: Technician Arrives
    CashLoading --> CashLoaded: Cash Cassettes Replaced
    CashLoaded --> Testing: System Self-Check
    Testing --> Online: All Checks Passed
    Testing --> Offline: Repair Needed
    CashLoaded --> CashLow: Cash Below Threshold
    CashLow --> CashLoading: Refill Triggered

    state Online {
        [*] --> Idle
        Idle --> TransactionActive: Customer Insert Card
        TransactionActive --> Idle: Session Complete
    end
```

---

## Aggregate Boundaries Identified

The Event Storming sessions revealed five distinct aggregate boundaries based on consistency requirements:

| Aggregate | Events Owned | Consistency Boundary Rationale |
|-----------|-------------|-------------------------------|
| **ATMSession** | SessionStarted, CardValidated, PinAuthenticated, SessionCompleted, SessionCancelled | Session lifecycle must be strictly sequential (state machine). High change frequency per session. Short-lived. |
| **DebitCard** | CardIssued, CardValidated, PinAuthenticated, PinAuthenticationFailed, CardConfiscated, CardBlocked | Card data (PIN, status, attempts) must be consistent. Global uniqueness of card number. Long-lived. |
| **Account** | AccountCreated, FundsWithdrawn, FundsDeposited, DailyLimitExceeded | Balance accuracy and daily limit enforcement require strong consistency. Concurrency protection for financial operations. Long-lived. |
| **ATM** | CashDispensed, CashLoaded | Cash inventory accuracy is critical. Physical device state (online/offline) must be consistent with capability. Long-lived. |
| **ATMTransaction** | TransactionCreated, TransactionCompleted, TransactionFailed | Audit trail for every financial operation. References other aggregates by ID, does not own their data. Append-only. |

---

## Coordination Between Aggregates

Aggregate coordination follows a **domain event → event handler** pattern rather than direct cross-aggregate calls:

```
ATMSession  --->  SessionCompletedDomainEvent
                      │
                      ▼
              Event Handler Orchestrator
                      │
          ┌───────────┼───────────┐
          ▼           ▼           ▼
    DebitCard     Account       ATM
    (update     (deduct      (dispense
     status)     funds)       cash)
```

**Key coordination rules:**
1. Within a single command handler, only one aggregate is modified directly.
2. Side effects on other aggregates are triggered by domain events published after `SaveChangesAsync`.
3. Compensating actions (e.g., reversing a withdrawal if cash dispensing fails) are implemented in event handlers.
4. The `TransactionBehavior` pipeline ensures atomicity within the primary aggregate; side effects are eventually consistent.

---

## Hot Spots and Open Questions

| ID | Hot Spot | Status | Notes |
|----|----------|--------|-------|
| **HS-01** | What happens if the ATM goes offline mid-session (after PIN auth but before cash dispensed)? | Open | Need session timeout policy. Should we auto-cancel sessions when ATM goes offline? |
| **HS-02** | Cash dispensing failure after funds withdrawn — how to reconcile? | Open | Currently no compensating transaction implemented. Account balance is debited but cash may not dispense. |
| **HS-03** | Daily limit reset — is it calendar day or rolling 24-hour window? | Decision Needed | Current implementation uses calendar day (UTC midnight). Should confirm with domain experts. |
| **HS-04** | Card confiscation race condition — customer removes card during PIN entry. | Open | The physical card ejection/retention mechanism is outside the software model. Need a timeout policy. |
| **HS-05** | Session timeout — how long should a session be idle before auto-cancellation? | Open | Not yet implemented. Recommendation: 30 seconds between steps, 2 minutes total. |
| **HS-06** | Transfer atomicity — should both accounts be debited/credited atomically, or is eventual consistency acceptable? | Open | Current design does not support transfers fully. If both accounts are in the same bounded context, consider a database transaction. |
| **HS-07** | Are "Deposit" transactions in scope for v1? | Open | Deposit functionality is mentioned in domain events but not yet implemented. |
| **HS-08** | Receipt printing — is this a domain concern or purely infrastructure? | Open | If receipt data contains business information, it should be modeled. Currently out of scope. |
| **HS-09** | How do we handle cash denomination logic? | Open | `CashDispenser` aggregate exists but denominations are not factored into dispensing logic. |
| **HS-10** | Domain event dispatching is currently commented out in infrastructure. | Known Issue | Events raised via `RaiseDomainEvent` are collected but not dispatched through event infrastructure. Only MediatR `INotification` works. |
