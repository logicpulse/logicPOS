using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Services;
using LogicPOS.Persistence.Database;
using LogicPOS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Authentication;

public sealed class LoginService : ILoginService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IPasswordHasher _passwordHasher;
    private Guid? _selectedTerminalId;

    public LoginService(IServiceScopeFactory scopes, IPasswordHasher passwordHasher)
    {
        _scopes = scopes;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UserOption>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var users = await database.Users
            .AsNoTracking()
            .Where(user => user.IsDeleted == false)
            .OrderBy(user => user.Code)
            .Select(user => new { user.Id, user.Name, user.PasswordReset })
            .ToListAsync(cancellationToken);

        return users
            .Select(user => new UserOption(user.Id, user.Name, user.PasswordReset))
            .ToList();
    }

    public async Task<string?> GetFirstTerminalNameAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        return await FindTerminalDesignationAsync(database, cancellationToken);
    }

    public async Task<Guid?> GetFirstTerminalIdAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        return await FindTerminalIdAsync(database, cancellationToken);
    }

    public async Task<TerminalLookup> LookupTerminalAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var matched = await FindTerminalAsync(database, cancellationToken);
        if (matched is not null)
        {
            return TerminalLookup.Matched(matched.Id, matched.Designation);
        }

        var terminals = await ListTerminalChoicesAsync(database, cancellationToken);
        return terminals.Count == 0 ? TerminalLookup.Missing() : TerminalLookup.Choose(terminals);
    }

    public void RememberTerminal(Guid terminalId, string name)
    {
        _selectedTerminalId = terminalId;
        _ = name;
    }

    public async Task<LoginAttemptResult> ClaimTerminalAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var terminal = await database.Terminals
            .FirstOrDefaultAsync(item => item.Id == terminalId && item.IsDeleted == false, cancellationToken);
        if (terminal is null)
        {
            return new LoginAttemptResult(false, "Terminal não encontrado");
        }

        terminal.HardwareId = MachineIdentity.HardwareId;
        terminal.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return new LoginAttemptResult(true, terminal.Designation);
    }

    public async Task<LoginAttemptResult> SignInAsync(Guid userId, string pin, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var terminalId = await FindTerminalIdAsync(database, cancellationToken);
        if (terminalId is null)
        {
            var choices = await ListTerminalChoicesAsync(database, cancellationToken);
            terminalId = choices.FirstOrDefault(item => item.IsDefault)?.Id ?? choices.FirstOrDefault()?.Id;
        }

        if (terminalId is null)
        {
            return new LoginAttemptResult(false, "Terminal não encontrado");
        }

        var user = await database.Users
            .AsNoTracking()
            .Where(candidate => candidate.Id == userId)
            .Select(candidate => new { candidate.Id, candidate.Name, candidate.IsDeleted, candidate.AccessPin })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return new LoginAttemptResult(false, "Utilizador não encontrado");
        }

        if (user.IsDeleted)
        {
            return new LoginAttemptResult(false, "Utilizador desactivado");
        }

        if (_passwordHasher.VerifyPassword(pin, user.AccessPin) == false)
        {
            return new LoginAttemptResult(false, "Credenciais inválidas");
        }

        if (_selectedTerminalId is Guid selectedId)
        {
            var claimed = await ClaimTerminalAsync(selectedId, cancellationToken);
            if (claimed.Succeeded == false)
            {
                return new LoginAttemptResult(false, claimed.Message) { TerminalUpdateFailed = true };
            }

            _selectedTerminalId = null;
        }

        return new LoginAttemptResult(true, user.Name);
    }

    public async Task<LoginAttemptResult> ReplaceDefaultPinAsync(
        Guid userId,
        string newPin,
        string? oldPin = null,
        CancellationToken cancellationToken = default)
    {
        if (newPin == User.DefaultAccessPin)
        {
            return new LoginAttemptResult(false, "O PIN 0000 tem de ser substituído.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var user = await database.Users.FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null)
        {
            return new LoginAttemptResult(false, "Utilizador não encontrado");
        }

        user.AccessPin = _passwordHasher.HashPassword(newPin);
        user.PasswordReset = false;
        user.PasswordResetDate = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return new LoginAttemptResult(true, user.Name);
    }

    private async Task<Guid?> FindTerminalIdAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var terminal = await FindTerminalAsync(database, cancellationToken);
        return terminal?.Id;
    }

    private async Task<string?> FindTerminalDesignationAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var terminal = await FindTerminalAsync(database, cancellationToken);
        return terminal?.Designation;
    }

    private async Task<TerminalMatch?> FindTerminalAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        if (_selectedTerminalId is Guid selectedId)
        {
            var selected = await database.Terminals
                .AsNoTracking()
                .Where(terminal => terminal.IsDeleted == false && terminal.Id == selectedId)
                .Select(terminal => new TerminalMatch(terminal.Id, terminal.Designation))
                .FirstOrDefaultAsync(cancellationToken);
            if (selected is not null)
            {
                return selected;
            }
        }

        var hardwareId = MachineIdentity.HardwareId;
        var match = await database.Terminals
            .AsNoTracking()
            .Where(terminal => terminal.IsDeleted == false && terminal.HardwareId == hardwareId)
            .Select(terminal => new TerminalMatch(terminal.Id, terminal.Designation))
            .FirstOrDefaultAsync(cancellationToken);

        return match;
    }

    private static async Task<IReadOnlyList<TerminalChoice>> ListTerminalChoicesAsync(
        LogicPOSDbContext database,
        CancellationToken cancellationToken)
    {
        var rows = await database.Terminals
            .AsNoTracking()
            .Where(terminal => terminal.IsDeleted == false)
            .OrderByDescending(terminal => terminal.IsDefault)
            .ThenBy(terminal => terminal.Code)
            .Select(terminal => new
            {
                terminal.Id,
                terminal.Code,
                terminal.Designation,
                terminal.IsDefault,
                terminal.HardwareId,
                terminal.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(terminal => new TerminalChoice(
                terminal.Id,
                terminal.Code ?? string.Empty,
                terminal.Designation ?? string.Empty,
                terminal.IsDefault,
                terminal.HardwareId ?? string.Empty,
                terminal.UpdatedAt))
            .ToList();
    }

    private sealed record TerminalMatch(Guid Id, string Designation);
}
