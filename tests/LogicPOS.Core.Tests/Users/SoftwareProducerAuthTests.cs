using System.Collections.Concurrent;
using System.Security.Cryptography;
using LogicPOS.Application.Features.System.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicPOS.Core.Tests.Users;

public sealed class SoftwareProducerAuthTests : IDisposable
{
    public SoftwareProducerAuthTests()
    {
        SoftwareProducerAuth.ResetConfiguration();
    }

    public void Dispose()
    {
        SoftwareProducerAuth.ResetConfiguration();
    }

    [Fact]
    public void Correct_password_matches()
    {
        var password = CreateThrowawayPassword();
        var verifier = SoftwareProducerPbkdf2.CreateVerifier(password);
        var (auth, _) = CreateConfigured(verifier);

        Assert.True(auth.VerifyPassword(password));
        SoftwareProducerAuth.Configure(auth);
        Assert.True(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, password));
    }

    [Fact]
    public void Wrong_password_fails()
    {
        var password = CreateThrowawayPassword();
        var verifier = SoftwareProducerPbkdf2.CreateVerifier(password);
        var (auth, _) = CreateConfigured(verifier);

        Assert.False(auth.VerifyPassword(password + "-wrong"));
        SoftwareProducerAuth.Configure(auth);
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, password + "-wrong"));
    }

    [Fact]
    public void Wrong_login_fails()
    {
        var password = CreateThrowawayPassword();
        var verifier = SoftwareProducerPbkdf2.CreateVerifier(password);
        var (auth, _) = CreateConfigured(verifier);
        SoftwareProducerAuth.Configure(auth);

        Assert.False(SoftwareProducerAuth.Matches("not-the-producer", password));
    }

    [Fact]
    public void No_verifier_configured_refuses_and_logs_warning()
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions()),
            logger);

        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Contains(logger.Messages, m =>
            m.Level == LogLevel.Warning
            && m.Text.Contains("not configured", StringComparison.OrdinalIgnoreCase));

        // Second attempt: still refuses, but only one warning.
        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Equal(1, logger.Messages.Count(m => m.Level == LogLevel.Warning));
    }

    [Fact]
    public void Malformed_verifier_fails_without_match()
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions { Verifier = "not-a-valid-verifier" }),
            logger);

        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Contains(logger.Messages, m => m.Level == LogLevel.Warning);
    }

    [Fact]
    public void Iterations_below_minimum_are_rejected()
    {
        var password = CreateThrowawayPassword();
        // Build a structurally valid string with too-few iterations (bypass CreateVerifier guard).
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var hash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var lowIter = $"pbkdf2-sha256$1000${salt}${hash}";

        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, lowIter));

        var (auth, logger) = CreateConfigured(lowIter);
        Assert.False(auth.VerifyPassword(password));
        Assert.Contains(logger.Messages, m => m.Level == LogLevel.Warning);
    }

    [Fact]
    public void Legacy_accepted_only_when_AcceptLegacy_is_true()
    {
        var password = CreateThrowawayPassword();
        var legacy = LegacyPasswordHasherFormat.HashPassword(password);

        var (enabled, _) = CreateConfigured(
            verifier: string.Empty,
            legacyVerifier: legacy,
            acceptLegacy: true);
        Assert.True(enabled.VerifyPassword(password));

        var (disabled, _) = CreateConfigured(
            verifier: string.Empty,
            legacyVerifier: legacy,
            acceptLegacy: false);
        Assert.False(disabled.VerifyPassword(password));
    }

    [Fact]
    public void Legacy_ignored_when_AcceptLegacy_is_false_even_if_legacy_string_set()
    {
        var password = CreateThrowawayPassword();
        var pbkdf2 = SoftwareProducerPbkdf2.CreateVerifier(password);
        var otherPassword = CreateThrowawayPassword();
        var legacy = LegacyPasswordHasherFormat.HashPassword(otherPassword);

        var (auth, _) = CreateConfigured(
            verifier: pbkdf2,
            legacyVerifier: legacy,
            acceptLegacy: false);

        Assert.True(auth.VerifyPassword(password));
        Assert.False(auth.VerifyPassword(otherPassword));
    }

    [Fact]
    public void Format_round_trip()
    {
        var password = CreateThrowawayPassword();
        var verifier = SoftwareProducerPbkdf2.CreateVerifier(password);

        Assert.StartsWith("pbkdf2-sha256$", verifier, StringComparison.Ordinal);
        var parts = verifier.Split('$');
        Assert.Equal(4, parts.Length);
        Assert.Equal(SoftwareProducerPbkdf2.DefaultIterations.ToString(), parts[1]);
        Assert.True(SoftwareProducerPbkdf2.TryVerify(password, verifier));
        Assert.False(SoftwareProducerPbkdf2.TryVerify(password + "x", verifier));
    }

    [Fact]
    public void Tool_hashing_function_produces_verifier_auth_accepts()
    {
        // Same API the CLI tool uses (SoftwareProducerPbkdf2.CreateVerifier).
        var password = CreateThrowawayPassword();
        var toolOutput = SoftwareProducerPbkdf2.CreateVerifier(password);
        var (auth, _) = CreateConfigured(toolOutput);

        Assert.True(auth.VerifyPassword(password));
        SoftwareProducerAuth.Configure(auth);
        Assert.False(SoftwareProducerAuth.Matches("other-user", password));
        Assert.True(SoftwareProducerAuth.Matches("LOGICPULSE", password));
    }

    [Fact]
    public void CreateVirtualUser_and_IsProducerUser_remain_compatible()
    {
        Assert.True(SoftwareProducerAuth.IsProducerUser(SoftwareProducerAuth.UserId));
        Assert.False(SoftwareProducerAuth.IsProducerUser(Guid.NewGuid()));

        var user = SoftwareProducerAuth.CreateVirtualUser();
        Assert.Equal(SoftwareProducerAuth.UserId, user.Id);
        Assert.Equal(SoftwareProducerAuth.Login, user.Login);
        Assert.Equal(SoftwareProducerAuth.DisplayName, user.Name);
    }

    private static string CreateThrowawayPassword() =>
        "test-" + Guid.NewGuid().ToString("N");

    private static (SoftwareProducerPasswordVerifier Auth, CollectingLogger<SoftwareProducerPasswordVerifier> Logger)
        CreateConfigured(
            string verifier,
            string? legacyVerifier = null,
            bool acceptLegacy = false)
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var options = new SoftwareProducerOptions
        {
            Verifier = verifier,
            LegacyVerifier = legacyVerifier,
            AcceptLegacy = acceptLegacy
        };
        var auth = new SoftwareProducerPasswordVerifier(new StaticOptionsMonitor(options), logger);
        return (auth, logger);
    }

    private sealed class StaticOptionsMonitor(SoftwareProducerOptions current) : IOptionsMonitor<SoftwareProducerOptions>
    {
        public SoftwareProducerOptions CurrentValue { get; } = current;

        public SoftwareProducerOptions Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<SoftwareProducerOptions, string?> listener) => null;
    }

    private sealed class CollectingLogger<T> : ILogger<T>
    {
        public ConcurrentBag<(LogLevel Level, string Text)> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add((logLevel, formatter(state, exception)));
        }
    }
}
