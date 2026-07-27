# Ubiquitous Language

> **Version:** 1.0  
> **Last Updated:** 2026-07-25  
> **Status:** Living Document

---

## Executive Summary

This document defines the Ubiquitous Language for the ATMSystem — a shared vocabulary used consistently by domain experts, developers, architects, testers, and all stakeholders. Every term in this document maps directly to code constructs (classes, methods, events, aggregates) in the implementation. Adherence to this language eliminates translation loss between business requirements and technical implementation.

The language is organized into actors, devices, banking terminology, value objects, aggregates, domain events, and business concepts. Each entry includes the term, its category, a precise definition, and its implementation mapping.

---

## Purpose

Ubiquitous Language is the cornerstone of Domain-Driven Design. In the ATMSystem project, it serves four critical purposes:

1. **Eliminate Translation Errors** — Business analysts, developers, and testers speak the same language, reducing misinterpretation of requirements.
2. **Drive Model Discovery** — Language ambiguities reveal missing concepts or incorrect boundaries (e.g., discovering that "Session" and "Transaction" are distinct aggregates).
3. **Anchor Code Design** — Every term appears as a class, method, property, or event in the codebase, ensuring the code reflects the mental model.
4. **Onboard New Team Members** — New developers learn the domain vocabulary alongside the codebase structure.

---

## Actors

| Term | Category | Definition | Code Mapping |
|------|----------|------------|--------------|
| **Customer** | Actor | The owner of one or more bank accounts who uses an ATM to perform financial operations. | Domain concept; referenced by `Account.AccountHolder` |
| **Bank Operator** | Actor | A bank employee who manages ATM operations including cash loading, maintenance scheduling, and card blocking. | Domain concept; initiates `BlockCard`, `LoadCash` commands |
| **Maintenance Technician** | Actor | A specialist who performs physical maintenance on ATM terminals (repairs, cash cassette replacement). | Domain concept; triggers `ATMStatus.Maintenance` transitions |
| **System** | Actor | The automated banking system that processes transactions, enforces business rules, and generates responses. | Implemented via MediatR command/query pipeline, domain event handlers |
| **ATM Terminal** | Device | A physical self-service banking machine that customers interact with to perform financial transactions. | `Banking.Domain.Aggregates.ATM` |
| **Debit Card** | Device | A physical plastic card issued by the bank containing a card number, expiration date, and associated PIN, used to authenticate a customer at an ATM. | `Banking.Domain.Cards.Aggregate.DebitCard` |
| **Card Reader** | Device | The hardware component of an ATM that reads the magnetic stripe or chip of a debit card when inserted. | Device concept; referenced in card insertion flow |
| **PIN Pad** | Device | The numeric keypad on an ATM where a customer enters their Personal Identification Number. | Device concept; captured by `AuthenticatePin` command |
| **Cash Dispenser** | Device | The hardware component of an ATM that holds and dispenses physical banknotes. | `Banking.Domain.Aggregates.CashDispenser` |
| **Receipt Printer** | Device | The hardware component of an ATM that prints transaction receipts. | Device concept; future implementation |

---

## Banking Terminology

| Term | Category | Definition | Code Mapping |
|------|----------|------------|--------------|
| **Account** | Banking | A financial account belonging to a customer, identified by a unique account number, with a balance, currency, daily limit, and status. | `Banking.Domain.Aggregates.Account` / `Bank.Server.Domain.AccountContext.Aggregates.Account` |
| **Balance** | Banking | The current amount of funds available in an account, denominated in a specific currency. | `Account.Balance` (type `Money`) |
| **Withdrawal** | Banking | The removal of funds from a customer's account through an ATM, subject to daily limits and sufficient funds checks. | `Account.Withdraw()` method; `WithdrawCommand` |
| **Deposit** | Banking | The addition of funds to a customer's account. | `AccountContext` domain concept; `FundsDepositedDomainEvent` |
| **Transfer** | Banking | The movement of funds from one account (source) to another account (destination). | Domain concept; `TransactionType.Withdrawal` and future `TransactionType.Transfer` |
| **Daily Limit** | Banking | The maximum amount of funds that can be withdrawn from an account in a single calendar day. | `Account.DailyLimit` (type `Money`) |
| **Currency** | Banking | The monetary unit in which an account or transaction is denominated (e.g., USD, EGP). | `BuildingBlocks.Domain.Common.Currency` / `Money.Currency` |
| **ATM ID** | Banking | A unique identifier assigned to each ATM terminal. | `ATMIdentifier` / `ATMId` |
| **Card Number** | Banking | A 16-digit number embossed on a debit card, used to identify the card and the associated account. Must pass Luhn validation. | `Banking.Domain.Cards.ValueObjects.CardNumber` |
| **PIN** | Banking | Personal Identification Number — a 4-to-6-digit secret code associated with a debit card, used to authenticate the cardholder. | `Banking.Domain.Cards.ValueObjects.Pin` |

---

## Value Objects

| Term | Category | Definition | Implementation |
|------|----------|------------|----------------|
| **CardNumber** | Value Object | A validated 16-digit card number that passes Luhn checksum validation. Exposes `LastFourDigits` for masked display. | `Banking.Domain.Cards.ValueObjects.CardNumber` |
| **Pin** | Value Object | A secret numeric code of 4–6 digits used for cardholder authentication. | `Banking.Domain.Cards.ValueObjects.Pin` |
| **ExpirationDate** | Value Object | The date on which a debit card expires. Includes `IsExpired` boolean check against current UTC date. | `Banking.Domain.Cards.ValueObjects.ExpirationDate` |
| **IssueDate** | Value Object | The date on which a debit card was issued to the customer. Must not be in the future. | `Banking.Domain.Cards.ValueObjects.IssueDate` |
| **TransactionNumber** | Value Object | A uniquely generated transaction reference number in the format `TXN-{yyyyMMddHHmmss}-{random}`. | `Banking.Domain.ATMSessions.ValueObjects.TransactionNumber` |
| **SessionId** | Value Object | A strongly-typed identifier wrapping a `Guid` for an ATM session. | `Banking.Domain.ATMSessions.ValueObjects.SessionId` |
| **ATMId** | Value Object | A strongly-typed identifier wrapping a `Guid` for an ATM terminal. | `Banking.Domain.ATMSessions.ValueObjects.ATMId` |
| **CardId** | Value Object | A strongly-typed identifier wrapping a `Guid` for a debit card. | `Banking.Domain.ATMSessions.ValueObjects.CardId` |
| **AccountId** | Value Object | (Referenced by `Guid` in aggregates) A unique identifier for a bank account. | `DebitCard.AccountId` (type `Guid`) |
| **AccountNumber** | Value Object | A string-based unique identifier for an account, distinct from the internal `Guid` ID. | `Bank.Server.Domain.AccountContext.ValueObjects.AccountNumber` |
| **Money** | Value Object | An amount of money with an associated currency code. Supports arithmetic operations (`Add`, `Subtract`) that enforce same-currency invariant. | `Bank.Server.Domain.AccountContext.ValueObjects.Money` |
| **ATMIdentifier** | Value Object | A string-based identifier for an ATM terminal (e.g., "ATM-001"). | `Bank.Server.Domain.ATMContext.ValueObjects.ATMIdentifier` |
| **Currency** | Value Object | A currency code (e.g., "USD", "EGP") with static factory instances. | `BuildingBlocks.Domain.Common.Currency` |

---

## Aggregates

| Term | Category | Definition | Implementation |
|------|----------|------------|----------------|
| **DebitCard** | Aggregate Root | Represents a physical bank card. Owns the card number, PIN, expiration date, issue date, status (Active/Blocked/Expired/Confiscated), and failed PIN attempt counter. Enforces card validation, PIN authentication, and confiscation rules. | `Banking.Domain.Cards.Aggregate.DebitCard` |
| **Account** | Aggregate Root | Represents a customer's bank account. Owns the balance, daily limit, withdrawn-today tracker, currency, and status. Enforces sufficient funds, daily limit, and currency match invariants. | `Banking.Domain.Aggregates.Account` / `Bank.Server.Domain.AccountContext.Aggregates.Account` |
| **ATM** | Aggregate Root | Represents an ATM terminal. Owns its identifier, location, operational status (Online/Offline/Maintenance), and cash inventory. | `Banking.Domain.Aggregates.ATM` / `Bank.Server.Domain.ATMContext.Aggregates.ATM` |
| **ATMTransaction** | Aggregate Root | Represents a financial transaction performed at an ATM. Owns the transaction amount, type, currency, status, and references to the ATM, account, and debit card. | `Banking.Domain.Aggregates.ATMTransaction` |
| **ATMSession** | Aggregate Root | Represents a customer's interactive session at an ATM from card insertion through completion or cancellation. Owns the session state machine (Started → CardValidated → PinAuthenticated → TransactionSelected → Completed/Cancelled). | `Banking.Domain.ATMSessions.Aggregate.ATMSession` |
| **Transaction** | Aggregate Root | (Legacy) Represents a financial transaction with lifecycle Pending → Approved → Completed/Cancelled. | `Bank.Server.Domain.TransactionContext.Aggregates.Transaction` |
| **CashDispenser** | Aggregate Root | Represents a cash cassette within an ATM with a specific denomination and note count. | `Banking.Domain.Aggregates.CashDispenser` |

---

## Domain Events

| Term | Category | Description | Fired By |
|------|----------|-------------|----------|
| **SessionStarted** | Domain Event | Raised when a new ATM session begins (customer present). | `ATMSession.Start()` |
| **CardValidated** | Domain Event | Raised when a debit card is successfully validated against the card database. | `ATMSession.ValidateCard()` |
| **PinAuthenticated** | Domain Event | Raised when the customer's PIN is successfully verified. | `ATMSession.Authenticate()` / `DebitCard.AuthenticatePin()` |
| **PinAuthenticationFailed** | Domain Event | Raised when an incorrect PIN is entered. Carries the current failed attempt count. | `DebitCard.AuthenticatePin()` |
| **SessionCompleted** | Domain Event | Raised when a session ends normally after a successful transaction. | `ATMSession.Complete()` |
| **SessionCancelled** | Domain Event | Raised when a session is terminated without completing a transaction (user cancel, timeout, max PIN attempts). | `ATMSession.Cancel()` / `ATMSession.Authenticate()` (on max attempts) |
| **CardConfiscated** | Domain Event | Raised when a card is retained by the ATM due to 3 failed PIN attempts or being reported lost/stolen. | `DebitCard.Confiscate()` / `DebitCard.AuthenticatePin()` / `DebitCard.IncrementFailedAttempts()` |
| **CardBlocked** | Domain Event | Raised when a card is blocked by a bank operator (e.g., reported stolen). | `DebitCard.Block()` |
| **CardIssued** | Domain Event | Raised when a new card is issued to a customer. | `DebitCard.Issue()` |
| **CardMarkedAsStolen** | Domain Event | Raised when a customer reports their card as stolen. | Bank operator action |
| **FundsWithdrawn** | Domain Event | Raised when funds are successfully withdrawn from an account. Carries amount, currency, ATM ID, and transaction reference. | `Account.Withdraw()` |
| **FundsDeposited** | Domain Event | Raised when funds are deposited into an account. | `Account` domain action |
| **DailyLimitExceeded** | Domain Event | Raised when a withdrawal would exceed the account's daily withdrawal limit. | `Account.Withdraw()` |
| **AccountCreated** | Domain Event | Raised when a new account is opened. | `Account.Create()` |
| **CashDispensed** | Domain Event | Raised when cash is dispensed from an ATM, reducing its cash inventory. | `ATM.DispenseCash()` |
| **CashLoaded** | Domain Event | Raised when cash is loaded into an ATM by a maintenance technician or bank operator. | `ATM.LoadCash()` |
| **TransactionApproved** | Domain Event | Raised when a pending transaction is approved. | `Transaction.Approve()` |
| **TransactionCompleted** | Domain Event | Raised when an approved transaction is marked complete. | `Transaction.Complete()` |
| **TransactionCancelled** | Domain Event | Raised when a pending transaction is cancelled. | `Transaction.Cancel()` |

---

## Business Concepts

| Term | Category | Definition |
|------|----------|------------|
| **Session** | Concept | An interactive period during which a customer uses an ATM. Begins when a card is inserted and ends when the card is ejected or confiscated. Manages state transitions through the session lifecycle. |
| **PIN Authentication** | Concept | The process of verifying a customer's identity by comparing the entered PIN against the stored PIN hash for the card. Tracks failed attempts and triggers confiscation after 3 failures. |
| **Card Validation** | Concept | The process of verifying that an inserted card is valid (not expired, not blocked, not stolen) before allowing further operations. |
| **Cash Dispensing** | Concept | The physical act of an ATM delivering cash notes to a customer. Requires the ATM to be online, have sufficient cash, and have the correct denominations. |
| **Card Confiscation** | Concept | The ATM retaining a card because the maximum number of failed PIN attempts (3) has been reached, or because the card has been reported lost or stolen. The card's status changes to Confiscated. |
| **Session State Machine** | Concept | The sequential lifecycle of an ATM session: Started → CardValidated → PinAuthenticated → TransactionSelected → Completed/Cancelled. Each transition is guarded by business rules. |
| **Transaction Lifecycle** | Concept | The lifecycle of a financial transaction: Pending → Approved → Completed (or Cancelled). A transaction must be approved before it can be completed. |
| **Daily Withdrawal Limit** | Concept | A per-account cap on the total amount that can be withdrawn in a single calendar day. Reset daily. |
| **Luhn Check** | Concept | A checksum algorithm used to validate card numbers. Implemented in the `CardNumber` value object to detect mistyped or invalid card numbers. |

---

## Language Consistency Rules

The following rules ensure consistent terminology across all communications, documentation, and code:

| Rule | Prefer | Avoid |
|------|--------|-------|
| "Validate Card" not "Check Card" | `ValidateCard` method, `CardValidated` event | `CheckCard`, `VerifyCard` |
| "Authenticate PIN" not "Verify PIN" | `AuthenticatePin` method, `PinAuthenticated` event | `VerifyPin`, `CheckPin` |
| "Confiscate Card" not "Keep Card" | `Confiscate()` method, `CardConfiscated` event | `KeepCard`, `RetainCard` |
| "Session" not "ATM Session" | `ATMSession` aggregate, `SessionStarted` event | `ATMSession` is acceptable, but "session" alone refers to ATM session |
| "Start Session" not "Begin Session" | `Start()` factory method, `StartSession` command | `BeginSession`, `CreateSession` |
| "Complete Transaction" not "Finish Transaction" | `Complete()` method, `TransactionCompleted` event | `FinishTransaction`, `EndTransaction` |
| "Cancel Session" not "Abort Session" | `Cancel()` method, `SessionCancelled` event | `AbortSession`, `TerminateSession` |
| "Eject Card" not "Return Card" | `EjectCard()` method | `ReturnCard`, `ReleaseCard` |
| "Load Cash" not "Add Cash" | `LoadCash()` method, `CashLoaded` event | `AddCash`, `RefillCash` |
| "Dispense Cash" not "Give Cash" | `DispenseCash()` method, `CashDispensed` event | `GiveCash`, `OutputCash` |
| "Account" not "Customer Account" | `Account` aggregate | `CustomerAccount` |
| "DebitCard" not "Card" | `DebitCard` aggregate (use specific type) | Bare `Card` (ambiguous) |
| "PIN" always uppercase | `Pin`, `AuthenticatePin` | `PinNumber`, `PinCode` |
| "Money" not "Amount" for typed value | `Money` value object | Bare `decimal Amount` (use `Money` type) |

---

## Glossary

| Term | Category | Description |
|------|----------|-------------|
| Account | Aggregate / Banking | A financial account belonging to a customer with balance, currency, and daily limit. |
| AccountNumber | Value Object | A unique string identifier for an account. |
| ATM | Aggregate / Device | An ATM terminal with cash inventory, location, and operational status. |
| ATMId | Value Object | Strongly-typed identifier for an ATM terminal. |
| ATMIdentifier | Value Object | A human-readable string identifier for an ATM. |
| ATMTransaction | Aggregate | A record of a financial transaction performed at an ATM. |
| ATMSession | Aggregate | An interactive session lifecycle at an ATM from card insert to eject. |
| AuthenticatePin | Command / Method | The action of verifying a customer's PIN against the stored value. |
| Balance | Value / Property | The available funds in an account. |
| Bank Operator | Actor | A bank employee managing ATM operations. |
| Cancel Session | Method / Command | The action of terminating an active session. |
| Card Blocked | Event / Status | A card status indicating the card has been blocked by the bank. |
| Card Confiscated | Event / Status | A card status indicating the ATM has retained the card. |
| Card Expired | Event / Status | A card status indicating the card has passed its expiration date. |
| Card Number | Value Object | A 16-digit number used to identify a debit card (Luhn-validated). |
| Card Reader | Device | ATM hardware component that reads card data. |
| CardId | Value Object | Strongly-typed identifier for a debit card. |
| CardValidated | Event | Raised when a card passes validation checks. |
| Cash Dispenser | Device | ATM hardware component that holds and dispenses banknotes. |
| CashDispensed | Event | Raised when cash is successfully dispensed. |
| CashLoaded | Event | Raised when cash is added to an ATM. |
| Currency | Value Object | A monetary unit (e.g., USD, EGP). |
| Customer | Actor | The account holder using the ATM. |
| Daily Limit | Business Rule | The maximum withdrawal amount per day for an account. |
| DailyLimitExceeded | Event | Raised when a withdrawal would exceed the daily limit. |
| DebitCard | Aggregate | A physical bank card with PIN, expiration, and status. |
| Deposit | Banking Concept | The addition of funds to an account. |
| DispenseCash | Method | The action of an ATM delivering cash notes. |
| Eject Card | Method | The action of returning a card to the customer after session end. |
| ExpirationDate | Value Object | The date beyond which a card is no longer valid. |
| Failed Attempts | Property | The count of consecutive failed PIN entries. |
| FundsWithdrawn | Event | Raised when funds are debited from an account. |
| IssueDate | Value Object | The date a card was issued. |
| LoadCash | Method | The action of adding cash to an ATM. |
| Luhn Check | Algorithm | Checksum validation for card numbers. |
| Maintenance Technician | Actor | A specialist who performs ATM repairs and maintenance. |
| Money | Value Object | An amount with currency, supporting arithmetic operations. |
| PIN | Value Object | A 4–6 digit secret code used for authentication. |
| PIN Pad | Device | ATM hardware component for PIN entry. |
| PinAuthenticated | Event | Raised when PIN verification succeeds. |
| PinAuthenticationFailed | Event | Raised when an incorrect PIN is entered. |
| Receipt Printer | Device | ATM hardware component that prints transaction receipts. |
| Session | Concept | The interactive period of ATM usage from card insert to eject. |
| SessionId | Value Object | Strongly-typed identifier for an ATM session. |
| SessionCancelled | Event | Raised when a session terminates abnormally. |
| SessionCompleted | Event | Raised when a session ends after a successful transaction. |
| SessionStarted | Event | Raised when a new ATM session begins. |
| Transaction | Aggregate | A record of a financial operation (withdrawal, deposit, inquiry). |
| Transaction Lifecycle | Concept | The state machine: Pending → Approved → Completed/Cancelled. |
| TransactionNumber | Value Object | A generated unique reference for a transaction. |
| Transfer | Banking Concept | Movement of funds between two accounts. |
| ValidateCard | Method | The action of checking card validity (expiration, status). |
| Withdrawal | Banking Concept | Removal of funds from an account via ATM. |
| WithdrawnToday | Property | The cumulative withdrawal amount for the current day. |
