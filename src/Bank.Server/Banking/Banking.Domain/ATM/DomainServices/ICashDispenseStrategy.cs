using Banking.Domain.ATM.ValueObjects;

namespace Banking.Domain.ATM.DomainServices;

public interface ICashDispenseStrategy
{
    DispensePlan CreatePlan(decimal amount, IReadOnlyCollection<CassetteInfo> availableCassettes);
}
