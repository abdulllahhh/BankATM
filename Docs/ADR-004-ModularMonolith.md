# ADR-004

Title:
Start with Modular Monolith

Status:
Accepted

## Context

The project is in its initial stage.

Business rules are not yet validated.

Microservices would increase deployment complexity.

## Decision

Implement a modular monolith.

Modules:

- Card
- Account
- Transaction
- ATM

Future extraction to microservices is planned.

## Consequences

Positive:

- Faster development
- Easier debugging
- Lower operational complexity

Negative:

- Single deployment unit
