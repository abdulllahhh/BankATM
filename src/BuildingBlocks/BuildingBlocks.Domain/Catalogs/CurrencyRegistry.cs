using BuildingBlocks.Domain.Services;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Catalogs;

public sealed class CurrencyRegistry : ICurrencyRegistry
{
    private readonly Dictionary<CurrencyCode, Currency> _currencies;

    private CurrencyRegistry(Dictionary<CurrencyCode, Currency> currencies)
    {
        _currencies = currencies;
    }

    public static CurrencyRegistry CreateDefault()
    {
        var usdCode = CurrencyCode.Create("USD");
        var eurCode = CurrencyCode.Create("EUR");
        var egpCode = CurrencyCode.Create("EGP");

        var currencies = new Dictionary<CurrencyCode, Currency>
        {
            { usdCode, Currency.Create(usdCode, "US Dollar", "$", 840, 2) },
            { eurCode, Currency.Create(eurCode, "Euro", "\u20AC", 978, 2) },
            { egpCode, Currency.Create(egpCode, "Egyptian Pound", "\u00A3", 818, 2) },
        };

        return new CurrencyRegistry(currencies);
    }

    public Currency Get(CurrencyCode code)
    {
        if (TryGet(code, out var currency))
        {
            return currency;
        }

        throw new KeyNotFoundException($"Currency with code '{code}' was not found in the registry.");
    }

    public bool TryGet(CurrencyCode code, out Currency currency)
    {
        return _currencies.TryGetValue(code, out currency!);
    }

    public IReadOnlyCollection<Currency> GetAll()
    {
        return _currencies.Values.ToList().AsReadOnly();
    }
}
