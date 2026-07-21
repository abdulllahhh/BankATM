namespace BuildingBlocks.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    private OutboxMessage(
        Guid id,
        Guid eventId,
        string type,
        string content,
        DateTime occurredOnUtc)
    {
        Id = id;
        EventId = eventId;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage FromDomainEvent(
        Guid eventId,
        string type,
        string content,
        DateTime occurredOnUtc)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event id must not be empty.", nameof(eventId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Message type must not be empty.", nameof(type));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content must not be empty.", nameof(content));

        return new OutboxMessage(
            Guid.NewGuid(),
            eventId,
            type,
            content,
            occurredOnUtc);
    }
}
