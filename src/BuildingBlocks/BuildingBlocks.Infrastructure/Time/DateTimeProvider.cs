using BuildingBlocks.Application.Abstractions.Time;

namespace BuildingBlocks.Infrastructure.Time;

/// <summary>
/// Provides the current UTC date and time by delegating to
/// <see cref="TimeProvider.System"/>. This abstraction enables
/// deterministic time in tests by replacing <see cref="TimeProvider"/>
/// at the composition root.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    private readonly TimeProvider _timeProvider;

    public DateTimeProvider()
        : this(TimeProvider.System)
    {
    }

    public DateTimeProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;
}
