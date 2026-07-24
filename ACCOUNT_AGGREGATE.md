# Account Aggregate - Comprehensive Analysis

## 1. Project Overview

The ATMSystem follows **Domain-Driven Design (DDD)** with a clean architecture consisting of:

- **`BuildingBlocks.Domain/`** - Shared domain primitives (base classes for `Entity`, `AggregateRoot`, `ValueObject`, `Result`, etc.)
- **`Bank.Server.Domain/`** - The domain model for the banking server, organized by bounded contexts (`AccountContext`, `CardContext`, `ATMContext`, `TransactionContext`, `AuditContext`)
- **`Bank.Server.Application/`** - Application layer with CQRS commands/queries and event handlers
- **`Bank.Server.Infrastructure/`** - Persistence (EF Core), repositories, domain event dispatching
- **`Bank.Server.Api/`** - REST API endpoints

---

## 2. What Is the Account Aggregate?

The **Account aggregate** is the core domain aggregate that models a **bank account** in the ATM system. It is the transactional consistency boundary for all account-related operations, primarily **cash withdrawals**.

**Location:** `src/Bank.Server/Bank.Server.Domain/AccountContext/Aggregates/Account.cs`

It inherits from `AggregateRoot<Guid>`, which gives it:
- An identity (`Id` of type `Guid`)
- A collection of `DomainEvents` that get dispatched when the aggregate is saved
- Entity equality semantics (two entities with the same ID are considered equal)

---

## 3. Class Hierarchy

```
Entity<TId>                       (BuildingBlocks.Domain.Common.Entity.cs)
  |
  +-- AggregateRoot<TId>          (BuildingBlocks.Domain.Common.AggregateRoot.cs)
  |     implements IAggregateRoot
  |     - List<IDomainEvent> _domainEvents
  |     - RaiseDomainEvent()
  |     - ClearDomainEvents()
  |
  +-- Account : AggregateRoot<Guid>   (Bank.Server.Domain.AccountContext.Aggregates.Account.cs)
```

---

## 4. Account Aggregate Structure

### 4.1 Properties (State)

The `Account` class holds five properties, all with `private set` to enforce encapsulation:

| Property | Type | Description |
|---|---|---|
| `AccountNumber` | `AccountNumber` (value object) | Unique identifier for the account |
| `Balance` | `Money` (value object) | Current available balance |
| `DailyLimit` | `Money` (value object) | Maximum total withdrawal amount per day |
| `WithdrawnToday` | `Money` (value object) | Cumulative amount withdrawn today |
| `Status` | `AccountStatus` (enum) | `Active` (only state currently defined) |

All monetary values use the `Money` value object to ensure type safety and prevent currency mismatch.

### 4.2 Factory Method: `Account.Create()`

```csharp
public static Account Create(AccountNumber accountNumber, Money openingBalance, Money dailyLimit)
```

- Sets `Id = Guid.NewGuid()`
- Initializes `WithdrawnToday` to zero in the same currency
- Sets `Status = AccountStatus.Active`
- Raises `AccountCreatedDomainEvent`

### 4.3 Core Behavior: `Account.Withdraw()`

```csharp
public Result Withdraw(Money amount, Guid atmId, Guid transactionId = default)
```

**Business rules enforced inside the aggregate:**

1. **ATM ID is required** - `atmId` must not be `Guid.Empty`
2. **Account must be active** - `Status` must be `Active`
3. **Currency match** - The withdrawal currency must match the account's balance currency
4. **Daily limit** - `WithdrawnToday + amount <= DailyLimit`
5. **Sufficient funds** - `Balance >= amount`

**On success:**
- `Balance = Balance.Subtract(amount)`
- `WithdrawnToday = WithdrawnToday.Add(amount)`
- Raises `FundsWithdrawnDomainEvent(AccountId, AtmId, Amount, Currency, TransactionId)`

**On daily limit exceeded:**
- Raises `DailyLimitExceededDomainEvent` (before returning failure)

---

## 5. Value Objects

The Account aggregate uses three value objects. All inherit from `ValueObject` and are compared by their components.

### 5.1 `AccountNumber`
- Wraps a `string Value`
- Created via `AccountNumber.Create(string value)` with validation (null/whitespace check)
- Equality based on `Value`

### 5.2 `Money`
- Wraps `decimal Amount` and `string Currency`
- Created via `Money.Create(decimal amount, string currency)` (amount must be >= 0)
- Methods: `Add(Money)`, `Subtract(Money)` - both enforce same currency
- Equality based on `Amount` and `Currency`

### 5.3 `AccountStatus`
- A simple enum with one value: `Active`
- (Easily extensible for `Frozen`, `Closed`, `Suspended`, etc.)

---

## 6. Domain Events

The aggregate raises these domain events:

| Event | Raised When |
|---|---|
| `AccountCreatedDomainEvent` | After `Create()` |
| `FundsWithdrawnDomainEvent` | After successful `Withdraw()` |
| `DailyLimitExceededDomainEvent` | When daily limit would be exceeded |
| `FundsDepositedDomainEvent` | Placeholder (not yet raised) |

All events inherit from `DomainEvent` (a base record) which implements `IDomainEvent` with:
- `Guid EventId` - auto-generated
- `DateTime OccurredOnUtc` - set to `DateTime.UtcNow`

---

## 7. Specifications (Business Rule Objects)

In the `Specifications/` folder, there are two standalone specification classes:

| File | Rule |
|---|---|
| `DailyLimitSpecification.cs` | `WithdrawnToday + amount <= DailyLimit` |
| `SufficientFundsSpecification.cs` | `Balance >= amount` |

These are **not currently used** by `Account.Withdraw()` (which inlines the checks), but they exist as reusable rule objects for potential use in other parts of the system.

---

## 8. How the Account Aggregate Fits into the Architecture

### 8.1 Repository Pattern

**Interface:** `IAccountRepository`
```csharp
Task<Account?> GetByAccountNumberAsync(AccountNumber accountNumber, CancellationToken cancellationToken);
Task SaveChangesAsync(CancellationToken cancellationToken);
```

**Implementation:** Uses `BankDbContext` and EF Core, queries `_context.Accounts` by `AccountNumber`.

### 8.2 EF Core Configuration

Maps the `Account` aggregate to the `Accounts` table:
- `AccountNumber` stored as a string column (has conversion) with unique index
- `Balance` stored as `Balance` (decimal) and `Currency` (varchar) via `OwnsOne`
- `DailyLimit` stored as `DailyLimit` and `DailyLimitCurrency`
- `WithdrawnToday` stored as `WithdrawnToday` and `WithdrawnTodayCurrency`
- `RowVersion` byte array for optimistic concurrency control

### 8.3 CQRS Application Flow

**Withdraw Command (example flow):**

1. **Controller** receives HTTP request
2. `WithdrawCommand` is sent through MediatR pipeline
3. **`TransactionBehavior` pipeline behavior** wraps execution in a database transaction
4. **`WithdrawCommandHandler`** executes:
   - Loads `Account` via `IAccountRepository.GetByAccountNumberAsync()`
   - Calls `account.Withdraw(amount, atmId)` on the domain model
   - Calls `_unitOfWork.SaveChangesAsync()`
5. **Unit of Work**:
   - Calls `_dbContext.SaveChangesAsync()` -- persists the modified Account and fires EF Core's change tracking
   - Dispatches domain events via `IDomainEventDispatcher`
   - Saves any additional changes from event handlers (e.g., audit logs)
6. **`TransactionBehavior`** commits the database transaction

### 8.4 Event Handlers (Side Effects)

When `FundsWithdrawnDomainEvent` is raised, three handlers respond:

| Handler | Side Effect |
|---|---|
| `FundsWithdrawnDomainEventHandler` | Creates an `AuditLog` entry |
| `BankingTransactionHandler` | Creates a `Transaction` record |
| `AtmCashInventoryHandler` | Calls `atm.DecreaseCashInventory()` |

This is a pure DDD pattern: the aggregate focuses on maintaining its own invariants, while side effects are handled by event handlers in the application layer.

---

## 9. Complete File Inventory

### Core Account Aggregate files

| File | Role |
|---|---|
| `Bank.Server.Domain/AccountContext/Aggregates/Account.cs` | The aggregate root itself |
| `Bank.Server.Domain/AccountContext/ValueObjects/AccountNumber.cs` | Value object: account number |
| `Bank.Server.Domain/AccountContext/ValueObjects/AccountStatus.cs` | Enum: account status values |
| `Bank.Server.Domain/AccountContext/ValueObjects/Money.cs` | Value object: amount + currency |
| `Bank.Server.Domain/AccountContext/DomainEvents/AccountCreatedDomainEvent.cs` | Domain event: account created |
| `Bank.Server.Domain/AccountContext/DomainEvents/FundsWithdrawnDomainEvent.cs` | Domain event: funds withdrawn |
| `Bank.Server.Domain/AccountContext/DomainEvents/FundsDepositedDomainEvent.cs` | Domain event: funds deposited (placeholder) |
| `Bank.Server.Domain/AccountContext/DomainEvents/DailyLimitExceededDomainEvent.cs` | Domain event: daily limit exceeded |
| `Bank.Server.Domain/AccountContext/Specifications/DailyLimitSpecification.cs` | Specification: daily limit rule |
| `Bank.Server.Domain/AccountContext/Specifications/SufficientFundsSpecification.cs` | Specification: sufficient funds rule |

### Base classes (BuildingBlocks)

| File | Role |
|---|---|
| `BuildingBlocks.Domain/Common/Entity.cs` | Base entity with ID and equality |
| `BuildingBlocks.Domain/Common/AggregateRoot.cs` | Base aggregate root with domain events |
| `BuildingBlocks.Domain/Common/IAggregateRoot.cs` | Aggregate root interface |
| `BuildingBlocks.Domain/Common/ValueObject.cs` | Base value object |
| `BuildingBlocks.Domain/Common/Result.cs` | Result type for domain operations |
| `BuildingBlocks.Domain/Events/IDomainEvent.cs` | Domain event interface |
| `BuildingBlocks.Domain/Specifications/ISpecification.cs` | Specification interface (expression-based) |
| `BuildingBlocks.Domain/Specifications/Specification.cs` | Abstract specification base |

### Application layer

| File | Role |
|---|---|
| `Bank.Server.Application/Abstractions/Persistence/IAccountRepository.cs` | Repository interface |
| `Bank.Server.Application/Features/Accounts/Withdraw/WithdrawCommand.cs` | CQRS command: withdraw |
| `Bank.Server.Application/Features/Accounts/Withdraw/WithdrawCommandHandler.cs` | Command handler |
| `Bank.Server.Application/Features/Accounts/Withdraw/WithdrawCommandValidator.cs` | FluentValidation validator |
| `Bank.Server.Application/Features/Accounts/GetBalance/GetBalanceQuery.cs` | CQRS query: get balance |
| `Bank.Server.Application/Features/Accounts/GetBalance/GetBalanceQueryHandler.cs` | Query handler |
| `Bank.Server.Application/Events/Handlers/FundsWithdrawnDomainEventHandler.cs` | Event handler: creates audit log |
| `Bank.Server.Application/Events/Handlers/BankingTransactionHandler.cs` | Event handler: creates transaction record |
| `Bank.Server.Application/Events/Handlers/AtmCashInventoryHandler.cs` | Event handler: updates ATM inventory |

### Infrastructure layer

| File | Role |
|---|---|
| `Bank.Server.Infrastructure/Persistence/BankDbContext.cs` | EF Core DbContext with `DbSet<Account>` |
| `Bank.Server.Infrastructure/Persistence/Configurations/AccountConfiguration.cs` | EF Core fluent mapping for Account |
| `Bank.Server.Infrastructure/Persistence/Repositories/AccountRepository.cs` | Repository implementation |
| `Bank.Server.Infrastructure/DomainEvent/DomainEventsExtractor.cs` | Extracts domain events from EF change tracker |
| `BuildingBlocks.Infrastructure/Persistence/UnitOfWork.cs` | Unit of Work that persists and dispatches events |
| `BuildingBlocks.Application/Behaviors/TransactionBehavior.cs` | MediatR pipeline: database transaction per command |

---

## 10. Summary

The **Account aggregate** is the transactional boundary for bank account operations in this ATM system. Key characteristics:

1. **Encapsulated state** -- All properties have `private set`; state changes only happen through behavior methods (`Withdraw()`)
2. **Invariant enforcement** -- Business rules (active status, sufficient funds, daily limit, currency match) are enforced inside `Withdraw()` before any state mutation
3. **Domain events for side effects** -- Successful operations raise events that are handled by application-layer handlers (audit logging, transaction recording, ATM cash inventory)
4. **Specifications as reusable rules** -- Standalone rule objects (`DailyLimitSpecification`, `SufficientFundsSpecification`) exist alongside the inline checks
5. **Value objects** -- `Money` and `AccountNumber` provide type safety and encapsulate validation
6. **Repository pattern** -- Persistence is abstracted behind `IAccountRepository`, with an EF Core implementation
7. **Unit of Work + TransactionBehavior** -- Ensures atomicity: database transaction + domain event dispatch + side-effect persistence all happen within a single transaction

The aggregate is designed to be mutated only through its own methods, loaded from a repository, modified in memory, then saved through a unit of work that persists changes and dispatches domain events -- a pure DDD aggregate pattern.
