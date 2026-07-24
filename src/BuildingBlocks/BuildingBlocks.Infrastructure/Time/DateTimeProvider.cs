using BuildingBlocks.Application.Abstractions.Time;

namespace BuildingBlocks.Infrastructure.Time;

/// <summary>
/// Provides the current UTC time by delegating to DateTime.UtcNow.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
