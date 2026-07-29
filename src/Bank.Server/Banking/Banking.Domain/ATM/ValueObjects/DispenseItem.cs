using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.ValueObjects;

namespace Banking.Domain.ATM.ValueObjects;

public sealed class DispenseItem : ValueObject
{
    public CassetteId CassetteId { get; }
    public Money Denomination { get; }
    public int BillCount { get; }

    public Money TotalValue => Denomination.Multiply(BillCount);

    private DispenseItem(CassetteId cassetteId, Money denomination, int billCount)
    {
        CassetteId = cassetteId;
        Denomination = denomination;
        BillCount = billCount;
    }

    public static DispenseItem Create(CassetteId cassetteId, Money denomination, int billCount)
    {
        if (cassetteId is null)
        {
            throw new DomainException("CassetteId is required.");
        }

        if (denomination is null)
        {
            throw new DomainException("Denomination is required.");
        }

        if (billCount <= 0)
        {
            throw new DomainException("Bill count must be greater than zero.");
        }

        return new DispenseItem(cassetteId, denomination, billCount);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CassetteId;
        yield return Denomination;
        yield return BillCount;
    }
}
