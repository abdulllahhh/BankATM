# Domain Model

## Card Aggregate

Responsibilities:

- Validate card status
- Validate PIN
- Track failed attempts

---

### Entity

Card

Attributes:

- CardNumber
- StartDate
- ExpiryDate
- Status
- FailedPinAttempts

---

### Value Objects

CardNumber
PIN

---

### Domain Events

CardValidated
PinValidationFailed
CardConfiscated

---
