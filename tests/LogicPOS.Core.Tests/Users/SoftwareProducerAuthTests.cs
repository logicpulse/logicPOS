using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using LogicPOS.Application.Features.System.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogicPOS.Core.Tests.Users;

public sealed class SoftwareProducerAuthTests : IDisposable
{
    public SoftwareProducerAuthTests()
    {
        SoftwareProducerAuth.ResetConfiguration();
        SoftwareProducerPasswordVerifier.ResetNotConfiguredLogForTests();
        SoftwareProducerPbkdf2.ResetBadVerifierLogForTests();
    }

    public void Dispose()
    {
        SoftwareProducerAuth.ResetConfiguration();
        SoftwareProducerPasswordVerifier.ResetNotConfiguredLogForTests();
        SoftwareProducerPbkdf2.ResetBadVerifierLogForTests();
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
    public void Matches_without_configuration_refuses_all_candidates()
    {
        SoftwareProducerAuth.ResetConfiguration();

        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, null));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, string.Empty));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, " "));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, CreateThrowawayPassword()));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, CreateThrowawayPassword()));

        // Empty / whitespace verifier must also refuse (fail-open mutation guard).
        var (emptyAuth, _) = CreateConfigured(string.Empty);
        SoftwareProducerAuth.Configure(emptyAuth);
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, CreateThrowawayPassword()));

        var (blankAuth, _) = CreateConfigured("   ");
        SoftwareProducerAuth.Configure(blankAuth);
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, CreateThrowawayPassword()));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, null));
        Assert.False(SoftwareProducerAuth.Matches(SoftwareProducerAuth.Login, string.Empty));
    }

    [Fact]
    public void No_verifier_configured_refuses_and_logs_warning_once_per_process()
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions()),
            logger);

        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Contains(logger.Messages, m =>
            m.Level == LogLevel.Warning
            && m.Text.Contains("not configured", StringComparison.OrdinalIgnoreCase));

        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Equal(1, logger.Messages.Count(m => m.Level == LogLevel.Warning));

        // Second instance shares the once-per-process flag.
        var logger2 = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth2 = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions()),
            logger2);
        Assert.False(auth2.VerifyPassword(CreateThrowawayPassword()));
        Assert.Empty(logger2.Messages);
    }

    [Fact]
    public void Malformed_verifier_fails_and_logs_warning_once_per_process()
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions { Verifier = "not-a-valid-verifier" }),
            logger);

        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.False(auth.VerifyPassword(CreateThrowawayPassword()));
        Assert.Equal(1, logger.Messages.Count(m => m.Level == LogLevel.Warning));
    }

    [Fact]
    public void Iterations_below_minimum_are_rejected()
    {
        var password = CreateThrowawayPassword();
        var lowIter = BuildRawVerifier(password, iterations: 1000, saltBytes: 16, hashBytes: 32);

        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, lowIter));

        var (auth, logger) = CreateConfigured(lowIter);
        Assert.False(auth.VerifyPassword(password));
        Assert.Contains(logger.Messages, m => m.Level == LogLevel.Warning);
    }

    [Fact]
    public void Iterations_above_maximum_are_rejected_without_deriving()
    {
        var password = CreateThrowawayPassword();
        var highIter = BuildRawVerifier(
            password,
            iterations: SoftwareProducerPbkdf2.MaximumIterations + 1,
            saltBytes: 16,
            hashBytes: 32);

        var sw = Stopwatch.StartNew();
        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, highIter));
        sw.Stop();

        // Must not run PBKDF2 at 10M+1 iterations (would take seconds).
        Assert.True(sw.ElapsedMilliseconds < 500, $"Expected fast reject, took {sw.ElapsedMilliseconds} ms");

        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var auth = new SoftwareProducerPasswordVerifier(
            new StaticOptionsMonitor(new SoftwareProducerOptions { Verifier = highIter }),
            logger);
        Assert.False(auth.VerifyPassword(password));
        Assert.Contains(logger.Messages, m =>
            m.Level == LogLevel.Warning
            && m.Text.Contains("maximum", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Salt_and_hash_size_minimums_are_enforced()
    {
        var password = CreateThrowawayPassword();

        var shortSalt = BuildRawVerifier(password, SoftwareProducerPbkdf2.DefaultIterations, saltBytes: 15, hashBytes: 32);
        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, shortSalt));

        var shortHash = BuildRawVerifier(password, SoftwareProducerPbkdf2.DefaultIterations, saltBytes: 16, hashBytes: 31);
        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, shortHash));

        var longHash = BuildRawVerifier(password, SoftwareProducerPbkdf2.DefaultIterations, saltBytes: 16, hashBytes: 33);
        Assert.False(SoftwareProducerPbkdf2.TryVerify(password, longHash));

        var exactMinimum = BuildRawVerifier(password, SoftwareProducerPbkdf2.DefaultIterations, saltBytes: 16, hashBytes: 32);
        Assert.True(SoftwareProducerPbkdf2.TryVerify(password, exactMinimum));
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
        var password = CreateThrowawayPassword();
        var toolOutput = SoftwareProducerPbkdf2.CreateVerifier(password);
        var (auth, _) = CreateConfigured(toolOutput);

        Assert.True(auth.VerifyPassword(password));
        SoftwareProducerAuth.Configure(auth);
        Assert.False(SoftwareProducerAuth.Matches("other-user", password));
        Assert.True(SoftwareProducerAuth.Matches("LOGICPULSE", password));
    }

    [Fact]
    public void Cli_rejects_redirected_input_with_clear_message()
    {
        using var error = new StringWriter();
        var exit = SoftwareProducerVerifierCliRules.ExitCodeIfInputRedirected(isInputRedirected: true, error);
        Assert.True(exit != 0);
        Assert.Contains("interactive", error.ToString(), StringComparison.OrdinalIgnoreCase);

        using var errorOk = new StringWriter();
        Assert.Equal(0, SoftwareProducerVerifierCliRules.ExitCodeIfInputRedirected(isInputRedirected: false, errorOk));
        Assert.Equal(string.Empty, errorOk.ToString());
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

    private static string BuildRawVerifier(string password, int iterations, int saltBytes, int hashBytes)
    {
        var salt = RandomNumberGenerator.GetBytes(saltBytes);
        byte[] hash;
        if (iterations > SoftwareProducerPbkdf2.MaximumIterations)
        {
            // Placeholder only — verify path must reject before deriving.
            hash = RandomNumberGenerator.GetBytes(hashBytes);
        }
        else
        {
            hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Math.Max(iterations, 1),
                HashAlgorithmName.SHA256,
                hashBytes);
        }

        return string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"pbkdf2-sha256${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}");
    }

    private static (SoftwareProducerPasswordVerifier Auth, CollectingLogger<SoftwareProducerPasswordVerifier> Logger)
        CreateConfigured(string verifier)
    {
        var logger = new CollectingLogger<SoftwareProducerPasswordVerifier>();
        var options = new SoftwareProducerOptions { Verifier = verifier };
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
