using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Exceptions;

public sealed class CurrencyMismatchException : DomainException
{
    public CurrencyCode Left { get; }
    public CurrencyCode Right { get; }

    public CurrencyMismatchException(CurrencyCode left, CurrencyCode right)
        : base($"Currency mismatch: cannot operate on {left} and {right}.")
    {
        Left = left;
        Right = right;
    }
}
