namespace BuildingBlocks.Application.Abstractions.Time;

/// <summary>
/// Provides the current UTC date and time, abstracting away system clock dependency.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}
