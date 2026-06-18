# ADR-003

Title:
Use CQRS

Status:
Accepted

## Context

ATM operations naturally separate:

Commands:

- Withdraw
- Transfer

Queries:

- Balance inquiry

## Decision

Use MediatR with CQRS.

Commands modify state.

Queries return data.

## Consequences

Positive:

- Clear use case organization
- Easier future scalability

Negative:

- Additional classes
