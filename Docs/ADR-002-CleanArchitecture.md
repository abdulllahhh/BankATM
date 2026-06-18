# ADR-002

Title:
Use Clean Architecture

Status:
Accepted

## Context

The system requires:

- Testability
- Separation of concerns
- Long-term maintainability

## Decision

Use four layers:

- Domain
- Application
- Infrastructure
- API

Dependencies point inward.

## Consequences

Positive:

- Easy testing
- Independent domain model

Negative:

- More projects and folders
