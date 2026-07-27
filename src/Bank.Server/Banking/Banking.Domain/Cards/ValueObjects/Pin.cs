using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Cards.ValueObjects;

public sealed class Pin : ValueObject
{
    public string Hash { get; }

    private Pin(string hash)
    {
        Hash = hash;
    }

    public static Pin Create(string plainPin)
    {
        if (string.IsNullOrWhiteSpace(plainPin))
        {
            throw new DomainException("PIN cannot be empty.");
        }

        if (plainPin.Length < 4 || plainPin.Length > 6)
        {
            throw new DomainException("PIN must be between 4 and 6 digits.");
        }

        if (!plainPin.All(char.IsDigit))
        {
            throw new DomainException("PIN must contain only digits.");
        }

        var hash = HashPin(plainPin);
        return new Pin(hash);
    }

    public static Pin FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new DomainException("PIN hash cannot be empty.");
        }

        return new Pin(hash);
    }

    public bool Matches(string plainPin)
    {
        var hash = HashPin(plainPin);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hash),
            Encoding.UTF8.GetBytes(Hash));
    }

    private static string HashPin(string pin)
    {
        var bytes = Encoding.UTF8.GetBytes(pin);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Hash;
    }
}
