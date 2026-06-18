# Context Map

## Overview

The Bank ATM System is divided into four bounded contexts:

1. Card Context
2. Account Context
3. Transaction Context
4. ATM Context

---

## Card Context

Responsibilities:

- Card validation
- PIN verification
- Card status management
- Failed PIN tracking
- Card confiscation

Aggregate Root:

- Card

---

## Account Context

Responsibilities:

- Account balance management
- Daily withdrawal limits
- Funds availability

Aggregate Root:

- Account

---

## Transaction Context

Responsibilities:

- Withdraw transactions
- Transfer transactions
- Balance inquiries
- Transaction lifecycle

Aggregate Root:

- Transaction

---

## ATM Context

Responsibilities:

- ATM cash inventory
- ATM operational status
- ATM maintenance operations

Aggregate Root:

- ATM

---

## Context Relationships

ATM Context -> Card Context

ATM Context -> Transaction Context

Transaction Context -> Account Context

Transaction Context -> Card Context
