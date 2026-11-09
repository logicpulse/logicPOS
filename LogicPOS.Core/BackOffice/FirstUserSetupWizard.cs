using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Services;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public interface IFirstUserSetupWizard
{
    Task<bool> NeedsSetupAsync(CancellationToken cancellationToken = default);

    Task<FirstUserSetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<FirstUserSetupResult> CreateAsync(FirstUserSetupRequest request, CancellationToken cancellationToken = default);
}

public sealed class FirstUserSetupDraft
{
    public IReadOnlyList<FirstUserProfileChoice> Profiles { get; init; } = [];

    public Guid? PreferredProfileId { get; init; }

    public string? BlockedReason { get; init; }
}

public sealed class FirstUserProfileChoice
{
    public Guid Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public override string ToString() => Label;
}

public sealed class FirstUserSetupRequest
{
    public Guid ProfileId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Login { get; init; } = string.Empty;

    public string AccessPin { get; init; } = string.Empty;

    public string AccessPinConfirm { get; init; } = string.Empty;
}

public sealed class FirstUserSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public static FirstUserSetupResult Done(string message) => new() { Ok = true, Message = message };

    public static FirstUserSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

public sealed class LocalFirstUserSetupWizard : IFirstUserSetupWizard
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IPasswordHasher _passwordHasher;

    public LocalFirstUserSetupWizard(IServiceScopeFactory scopes, IPasswordHasher passwordHasher)
    {
        _scopes = scopes;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> NeedsSetupAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.Users.AsNoTracking()
            .AnyAsync(user => user.IsDeleted == false, cancellationToken) == false;
    }

    public async Task<FirstUserSetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var profiles = await database.UserProfiles.AsNoTracking()
            .Where(profile => profile.IsDeleted == false)
            .OrderBy(profile => profile.Order)
            .ThenBy(profile => profile.Designation)
            .Select(profile => new FirstUserProfileChoice
            {
                Id = profile.Id,
                Label = profile.Designation
            })
            .ToListAsync(cancellationToken);

        if (profiles.Count == 0)
        {
            return new FirstUserSetupDraft
            {
                BlockedReason = "Não existem perfis de utilizador. Volte a instalar ou reponha a semente da base de dados."
            };
        }

        var preferred = profiles.FirstOrDefault(profile =>
                profile.Label.Contains("Administr", StringComparison.OrdinalIgnoreCase)
                || profile.Label.Contains("Propriet", StringComparison.OrdinalIgnoreCase)
                || profile.Label.Contains("Owner", StringComparison.OrdinalIgnoreCase))
            ?? profiles[0];

        return new FirstUserSetupDraft
        {
            Profiles = profiles,
            PreferredProfileId = preferred.Id
        };
    }

    public async Task<FirstUserSetupResult> CreateAsync(
        FirstUserSetupRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var login = request.Login.Trim();
        var pin = request.AccessPin.Trim();
        var pinConfirm = request.AccessPinConfirm.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return FirstUserSetupResult.Fail("Indique o nome do utilizador.");

        if (string.IsNullOrWhiteSpace(login))
            return FirstUserSetupResult.Fail("Indique o login.");

        if (pin.Length < 4 || pin.Any(ch => char.IsDigit(ch) == false))
            return FirstUserSetupResult.Fail("O PIN deve ter pelo menos 4 dígitos.");

        if (pin == User.DefaultAccessPin)
            return FirstUserSetupResult.Fail("Escolha um PIN diferente de 0000.");

        if (pin != pinConfirm)
            return FirstUserSetupResult.Fail("A confirmação do PIN não coincide.");

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        if (await database.Users.AsNoTracking().AnyAsync(user => user.IsDeleted == false, cancellationToken))
            return FirstUserSetupResult.Fail("Já existe pelo menos um utilizador.");

        var profileExists = await database.UserProfiles.AsNoTracking()
            .AnyAsync(profile => profile.Id == request.ProfileId && profile.IsDeleted == false, cancellationToken);
        if (profileExists == false)
            return FirstUserSetupResult.Fail("Seleccione um perfil válido.");

        if (await database.Users.AsNoTracking()
                .AnyAsync(user => user.Login == login, cancellationToken))
            return FirstUserSetupResult.Fail("Esse login já existe.");

        var nextOrder = await database.Users.AsNoTracking()
            .Select(user => (uint?)user.Order)
            .MaxAsync(cancellationToken) ?? 0;

        database.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Order = nextOrder + 10,
            Code = "1",
            Name = name,
            Login = login,
            AccessPin = _passwordHasher.HashPassword(pin),
            PasswordReset = false,
            PasswordResetDate = DateTime.Now,
            ProfileId = request.ProfileId,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });

        await database.SaveChangesAsync(cancellationToken);
        return FirstUserSetupResult.Done($"Utilizador '{name}' criado. Pode entrar com o PIN definido.");
    }
}