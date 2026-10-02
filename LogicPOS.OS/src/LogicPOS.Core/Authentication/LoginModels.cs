namespace LogicPOS.Core.Authentication;

public sealed class UserOption
{
    public UserOption(Guid id, string name, bool requiresPasswordReset)
    {
        Id = id;
        Name = name;
        RequiresPasswordReset = requiresPasswordReset;
    }

    public Guid Id { get; }

    public string Name { get; }

    public bool RequiresPasswordReset { get; set; }

    public override string ToString() => Name;
}

public sealed record LoginAttemptResult(bool Succeeded, string Message)
{
    public bool RequiresNormalLogin { get; init; }

    public bool TerminalUpdateFailed { get; init; }
}
