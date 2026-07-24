using BuildingBlocks.Application.Abstractions.Time;

namespace BuildingBlocks.Infrastructure.Persistence.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
