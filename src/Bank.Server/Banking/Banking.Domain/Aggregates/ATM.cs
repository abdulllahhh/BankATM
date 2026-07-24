using BuildingBlocks.Domain.Common;

namespace Banking.Domain.Aggregates;

public sealed class ATM : AggregateRoot<Guid>
{
    public string Identifier { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public ATMStatus Status { get; private set; }
    public DateTime? LastMaintenance { get; private set; }

    private ATM() { }

    public ATM(Guid id, string identifier, string location)
        : base(id)
    {
        Identifier = identifier;
        Location = location;
        Status = ATMStatus.Online;
    }
}

public enum ATMStatus
{
    Online,
    Offline,
    Maintenance
}
