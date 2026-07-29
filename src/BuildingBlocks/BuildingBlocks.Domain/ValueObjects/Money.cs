using BuildingBlocks.Domain.Common;

namespace BuildingBlocks.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();

    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, Currency currency)
    {
        if (currency is null)
        {
            throw new DomainException("Currency is required.");
        }

        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency)
    {
        return new Money(0, currency);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public Money Negate()
    {
        return new Money(-Amount, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        EnsureSameCurrency(other);
        return Amount < other.Amount;
    }

    public bool IsZero()
    {
        return Amount == 0;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (!Currency.Equals(other.Currency))
        {
            throw new DomainException(
                $"Currency mismatch: cannot operate on {Currency.Code} and {other.Currency.Code}.");
        }
    }

    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return money.Multiply(multiplier);
    }

    public static Money operator *(decimal multiplier, Money money)
    {
        return money.Multiply(multiplier);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }

    public static bool operator >(Money left, Money right)
    {
        return left.IsGreaterThan(right);
    }

    public static bool operator <(Money left, Money right)
    {
        return left.IsLessThan(right);
    }

    public static bool operator >=(Money left, Money right)
    {
        return !left.IsLessThan(right);
    }

    public static bool operator <=(Money left, Money right)
    {
        return !left.IsGreaterThan(right);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
