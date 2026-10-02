namespace LogicPOS.Core.Authentication;

public sealed class TerminalChoice
{
    public TerminalChoice(Guid id, string code, string designation, bool isDefault, string hardwareId, DateTime? updatedAt)
    {
        Id = id;
        Code = code;
        Designation = designation;
        IsDefault = isDefault;
        HardwareId = hardwareId;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    public string Code { get; }

    public string Designation { get; }

    public bool IsDefault { get; }

    public string HardwareId { get; }

    public DateTime? UpdatedAt { get; }
}

public sealed class TerminalLookup
{
    public Guid? TerminalId { get; init; }

    public string? Designation { get; init; }

    public bool NeedsSelection { get; init; }

    public IReadOnlyList<TerminalChoice> Terminals { get; init; } = [];

    public static TerminalLookup Matched(Guid id, string? designation) =>
        new() { TerminalId = id, Designation = designation };

    public static TerminalLookup Choose(IReadOnlyList<TerminalChoice> terminals) =>
        new() { NeedsSelection = true, Terminals = terminals };

    public static TerminalLookup Missing() => new();
}
