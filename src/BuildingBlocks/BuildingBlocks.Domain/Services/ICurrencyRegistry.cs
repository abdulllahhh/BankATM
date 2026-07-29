using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Services;

public interface ICurrencyRegistry
{
    Currency Get(CurrencyCode code);

    bool TryGet(CurrencyCode code, out Currency currency);

    IReadOnlyCollection<Currency> GetAll();
}
