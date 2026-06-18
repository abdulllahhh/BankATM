# ADR-001

Title:
Adopt Domain Driven Design

Status:
Accepted

Date:
2026-06-18

## Context

The ATM system contains complex business rules:

- Card validation
- Transaction processing
- Account constraints

These rules must remain independent of infrastructure concerns.

## Decision

The project will use Domain Driven Design.

Bounded contexts:

- Card
- Account
- Transaction
- ATM

## Consequences

Positive:

- Clear business boundaries
- Easier migration to microservices

Negative:

- More initial complexity
