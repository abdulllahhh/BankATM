using System.Diagnostics;
using BuildingBlocks.Application.Abstractions.Messaging;

namespace BuildingBlocks.Infrastructure.Correlation;

/// <summary>
/// Resolves the correlation identifier for the current request scope.
/// Reads <see cref="Activity.Current"/> first (distributed tracing),
/// then falls back to a per-request <see cref="AsyncLocal{T}"/> value
/// generated at scope creation.
/// </summary>
public sealed class CorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<CorrelationHolder> _fallback = new();

    public CorrelationContext()
    {
        _fallback.Value ??= new CorrelationHolder
        {
            Id = Guid.NewGuid().ToString("D")
        };
    }

    public string CorrelationId
    {
        get
        {
            var activityId = Activity.Current?.RootId;
            if (activityId is not null)
            {
                return activityId;
            }

            return _fallback.Value?.Id ?? string.Empty;
        }
    }

    private sealed class CorrelationHolder
    {
        public string Id { get; set; } = string.Empty;
    }
}
