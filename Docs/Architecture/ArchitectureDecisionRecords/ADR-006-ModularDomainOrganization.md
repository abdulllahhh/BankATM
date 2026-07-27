# ADR-006

**Title:** Modular Domain Organization

**Status:** Accepted

**Date:** 2026-07-25

## Context

The initial domain model was organized with all aggregates in a flat `Banking.Domain/Aggregates/` folder:

```
Banking.Domain/
  Aggregates/
    Account.cs
    ATM.cs
    ATMTransaction.cs
    CashDispenser.cs
    DebitCard.cs
```

As the domain grows, this flat structure creates several problems:

1. **Cognitive load:** As new aggregates are added (e.g., `ATMSession`), it becomes increasingly difficult to understand which aggregates belong together and how they relate.
2. **Naming collisions:** Value objects, enums, and domain events must be globally unique within the namespace. For example, `TransactionType` appears in both the Transaction and ATMSession contexts with different semantic meanings, forcing awkward naming or namespace prefixing.
3. **Unclear boundaries:** Without explicit module folders, there is no visual cue in the file system about which aggregates form a cohesive business module.
4. **Harder extraction:** When a bounded context is extracted to a microservice, the flat structure makes it harder to identify which files belong together. Every file must be manually inspected rather than simply copying a folder.

## Decision

We will organize domain aggregates into **feature‑aligned modules** under `Banking.Domain/`. Each module corresponds to a bounded context or a distinct sub‑domain and has its own sub‑folder with a standardized internal structure.

### Module Folder Structure

Each module follows this convention:

```
Banking.Domain/
  {ModuleName}/
    Aggregate/         — Aggregate root(s) for this module
    ValueObjects/      — Value objects owned by this module
    Events/            — Domain events produced by this module
    Enums/             — Enum types used by this module
    Errors/            — Static error message classes
```

### Module Namespaces

Each module uses its own namespace to enforce compile‑time boundaries:

| Module | Namespace | Example |
|---|---|---|
| Cards | `Banking.Domain.Cards.*` | `Banking.Domain.Cards.Aggregate.DebitCard` |
| ATMSessions | `Banking.Domain.ATMSessions.*` | `Banking.Domain.ATMSessions.Aggregate.ATMSession` |
| *(future)* Account | `Banking.Domain.Accounts.*` | `Banking.Domain.Accounts.Aggregate.Account` |
| *(future)* Transaction | `Banking.Domain.Transactions.*` | `Banking.Domain.Transactions.Aggregate.ATMTransaction` |
| *(future)* ATM | `Banking.Domain.ATMs.*` | `Banking.Domain.ATMs.Aggregate.ATM` |

### Current State: Cards and ATMSessions (Migrated)

The **Cards** module was the first to be migrated:

```
Banking.Domain/
  Cards/
    Aggregate/
      DebitCard.cs                    # Aggregate root
    ValueObjects/
      CardNumber.cs                   # Luhn‑validated 16‑digit card number
      Pin.cs                          # 4–6 digit PIN
      ExpirationDate.cs               # Card expiry with IsExpired guard
      IssueDate.cs                    # Card issuance date
    Events/
      CardValidatedDomainEvent.cs
      PinAuthenticatedDomainEvent.cs
      PinAuthenticationFailedDomainEvent.cs
      CardConfiscatedDomainEvent.cs
      CardBlockedDomainEvent.cs
    Enums/
      CardStatus.cs                   # Active, Blocked, Expired, Confiscated
    Errors/
      CardErrors.cs                   # All card‑related error messages
```

The **ATMSessions** module was created fresh (per ADR-005):

```
Banking.Domain/
  ATMSessions/
    Aggregate/
      ATMSession.cs                   # Aggregate root (state machine)
    ValueObjects/
      SessionId.cs                    # Strongly‑typed Guid
      ATMId.cs                        # Strongly‑typed Guid
      CardId.cs                       # Strongly‑typed Guid
      TransactionNumber.cs            # Generated TXN-{ts}-{random}
    Events/
      SessionStartedDomainEvent.cs
      CardValidatedDomainEvent.cs
      PinAuthenticatedDomainEvent.cs
      SessionCompletedDomainEvent.cs
      SessionCancelledDomainEvent.cs
    Enums/
      SessionStatus.cs                # Started → ... → Completed/Cancelled
      TransactionType.cs              # Withdrawal, Deposit, BalanceInquiry
    Errors/
      SessionErrors.cs                # All session‑related error messages
```

### Legacy Flat Aggregates (Not Yet Migrated)

The following aggregates remain in the flat `Aggregates/` folder and will be progressively migrated to their own modules:

```
Banking.Domain/
  Aggregates/                          # ← Legacy — pending migration
    Account.cs
    ATM.cs
    ATMTransaction.cs
    CashDispenser.cs
```

### Migration Plan

| Priority | Aggregate | Target Module | Reason |
|---|---|---|---|
| 1 | `Account.cs` | `Banking.Domain.Accounts/` | Core aggregate with most business rules |
| 2 | `ATMTransaction.cs` | `Banking.Domain.Transactions/` | Distinct lifecycle, separate from ATM |
| 3 | `ATM.cs` + `CashDispenser.cs` | `Banking.Domain.ATMs/` | ATM + dispensers form one module |

For each migration step:

1. Create the module folder structure (`Aggregate/`, `ValueObjects/`, `Events/`, `Enums/`, `Errors/`)
2. Move the aggregate class into `Aggregate/`
3. Extract value objects into `ValueObjects/` (e.g., move `TransactionType` enum to module‑specific enum)
4. Extract domain events into `Events/`
5. Update namespaces
6. Update all cross‑references in the Application and Infrastructure layers
7. Run the full test suite to confirm no regressions

### Cross‑Module References

Modules may reference value objects from other modules when necessary. For example, `ATMSession` references `CardId` (a `ValueObject` in ATMSessions module) which represents the `DebitCard` aggregate from the Cards module. This is intentional — the reference is by identity, not by aggregate reference.

To keep coupling minimal:

- Modules reference each other by **identity** (value objects like `CardId`, `ATMId`), not by aggregate reference
- Domain events from one module can reference value objects from another module by value
- Cross‑module business logic is orchestrated through the Application layer, not through direct domain references

## Consequences

### Positive

- **Clear module boundaries:** The folder structure makes it immediately obvious which aggregates belong to which bounded context.
- **Easier extraction:** When a bounded context is promoted to a microservice, the entire module folder can be copied with its namespace intact.
- **No naming collisions:** Each module owns its own namespace, so `TransactionType` in `ATMSessions` and `TransactionType` in `Transactions` (future) can coexist without conflict.
- **Progressive migration:** The flat `Aggregates/` folder holds legacy code; new aggregates follow the new convention. Migration happens incrementally.
- **Consistent discoverability:** Every module follows the same sub‑folder pattern. Developers know where to find value objects, events, or errors in any module.
- **Ubiquitous language alignment:** The module names directly correspond to the ubiquitous language terms defined in the project glossary.

### Negative

- **More folders:** Each module adds 5+ sub‑folders. A project with 5 modules will have 25+ additional folders in the Domain project.
- **Cross‑module references:** Some value objects may need to be duplicated or shared across modules. For example, `CardId` in the ATMSessions module is semantically identical to the identity of `DebitCard` in the Cards module. This duplication is acceptable to maintain module independence.
- **Migration effort:** Migrating existing aggregates from the flat `Aggregates/` folder requires updating namespace references across the Application, Infrastructure, and test projects.
- **Namespace churn:** During the migration period, there will be a mix of `Banking.Domain.Aggregates.*` and `Banking.Domain.{Module}.*` namespaces, which may confuse developers.

### Mitigations

- Cross‑module value object sharing is documented in the module's README (or in code comments) to clarify that `CardId` in ATMSessions is a reference to the Cards module's `DebitCard` identity.
- The migration is tracked in the project backlog with clear acceptance criteria for each module.
- A `.editorconfig` or `Directory.Build.props` rule may be added to warn when new files are created under the legacy `Aggregates/` folder, encouraging use of the module structure.

## Compliance

- This ADR is consistent with **ADR-001 (DDD)** — modules align with bounded contexts and bounded context boundaries are explicit at the file system level.
- This ADR is consistent with **ADR-004 (Modular Monolith)** — the module structure is designed so that each module can be extracted to a microservice by copying the folder and updating the project/solution files.
- This ADR is consistent with **ADR-005 (Session Management)** — ATMSessions module was the first module created under this new convention.

## References

- ADR-001: Domain Driven Design — Bounded contexts definition
- ADR-004: Modular Monolith — Extraction readiness
- ADR-005: Session Management Aggregate Design — First module implemented under this structure
- `Banking.Domain.Cards/` — Migrated module (Cards)
- `Banking.Domain.ATMSessions/` — New module (ATMSessions)
- `Banking.Domain.Aggregates/` — Legacy flat folder (pending migration)
