namespace LogicPOS.Core.Authentication;

public interface ILoginService
{
    Task<IReadOnlyList<UserOption>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<string?> GetFirstTerminalNameAsync(CancellationToken cancellationToken = default);

    Task<Guid?> GetFirstTerminalIdAsync(CancellationToken cancellationToken = default);

    Task<TerminalLookup> LookupTerminalAsync(CancellationToken cancellationToken = default);

    void RememberTerminal(Guid terminalId, string name);

    Task<LoginAttemptResult> ClaimTerminalAsync(Guid terminalId, CancellationToken cancellationToken = default);

    Task<LoginAttemptResult> SignInAsync(Guid userId, string pin, CancellationToken cancellationToken = default);

    Task<LoginAttemptResult> ReplaceDefaultPinAsync(Guid userId, string newPin, string? oldPin = null, CancellationToken cancellationToken = default);
}
