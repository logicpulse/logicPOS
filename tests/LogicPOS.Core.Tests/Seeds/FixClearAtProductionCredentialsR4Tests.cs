using System.Security.Cryptography;
using System.Text;
using LogicPOS.Core;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.DataFixes;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests.Seeds;

[Collection(LogicPOS.Core.Tests.AppCompositionCollection.Name)]
public sealed class FixClearAtProductionCredentialsR4Tests
{
    /// <summary>Fictitious password used only in tests — never a production seed value.</summary>
    private const string FakePassword = "unit-test-at-password-NOT-REAL";

    /// <summary>Fictitious fiscal number used only in tests — never a production seed value.</summary>
    private const string FakeFiscalNumber = "UT-FISCAL-0001";

    private static readonly string FakePasswordSha256 = Sha256Hex(FakePassword);
    private static readonly string FakeFiscalSha256 = Sha256Hex(FakeFiscalNumber);

    [Fact]
    public async Task Apply_clears_only_when_stored_value_hash_matches_injectable_digest()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-at-cred-clear-");
        try
        {
            ConfigureApp(root);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var password = await database.PreferenceParameters
                .SingleAsync(item => item.Token == FixClearAtProductionCredentialsR4.PasswordToken);
            var fiscal = await database.PreferenceParameters
                .SingleAsync(item => item.Token == FixClearAtProductionCredentialsR4.FiscalNumberToken);

            password.Value = FakePassword;
            fiscal.Value = FakeFiscalNumber;
            await database.SaveChangesAsync();

            var cleared = FixClearAtProductionCredentialsR4.Apply(
                database,
                passwordValueSha256Hex: FakePasswordSha256,
                fiscalNumberValueSha256Hex: FakeFiscalSha256);

            Assert.Equal(2, cleared);
            await database.Entry(password).ReloadAsync();
            await database.Entry(fiscal).ReloadAsync();
            Assert.Equal(string.Empty, password.Value);
            Assert.Equal(string.Empty, fiscal.Value);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Apply_keeps_customer_value_when_hash_does_not_match()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-at-cred-keep-");
        try
        {
            ConfigureApp(root);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var password = await database.PreferenceParameters
                .SingleAsync(item => item.Token == FixClearAtProductionCredentialsR4.PasswordToken);
            password.Value = "customer-chosen-secret-NOT-SEED";
            await database.SaveChangesAsync();

            var cleared = FixClearAtProductionCredentialsR4.Apply(
                database,
                passwordValueSha256Hex: FakePasswordSha256,
                fiscalNumberValueSha256Hex: FakeFiscalSha256);

            Assert.Equal(0, cleared);
            await database.Entry(password).ReloadAsync();
            Assert.Equal("customer-chosen-secret-NOT-SEED", password.Value);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Apply_is_noop_when_values_already_empty()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-fix-at-cred-empty-");
        try
        {
            ConfigureApp(root);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

            var password = await database.PreferenceParameters
                .SingleAsync(item => item.Token == FixClearAtProductionCredentialsR4.PasswordToken);
            var fiscal = await database.PreferenceParameters
                .SingleAsync(item => item.Token == FixClearAtProductionCredentialsR4.FiscalNumberToken);

            Assert.True(string.IsNullOrEmpty(password.Value));
            Assert.True(string.IsNullOrEmpty(fiscal.Value));

            var cleared = FixClearAtProductionCredentialsR4.Apply(database);
            Assert.Equal(0, cleared);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void Sha256Hex_of_fictitious_values_matches_injectable_constants()
    {
        Assert.Equal(FakePasswordSha256, FixClearAtProductionCredentialsR4.Sha256Hex(FakePassword));
        Assert.Equal(FakeFiscalSha256, FixClearAtProductionCredentialsR4.Sha256Hex(FakeFiscalNumber));
        Assert.Equal(64, FixClearAtProductionCredentialsR4.DefaultPasswordValueSha256Hex.Length);
        Assert.Equal(64, FixClearAtProductionCredentialsR4.DefaultFiscalNumberValueSha256Hex.Length);
    }

    [Fact]
    public void Default_digests_are_lowercase_hex()
    {
        Assert.Matches("^[0-9a-f]{64}$", FixClearAtProductionCredentialsR4.DefaultPasswordValueSha256Hex);
        Assert.Matches("^[0-9a-f]{64}$", FixClearAtProductionCredentialsR4.DefaultFiscalNumberValueSha256Hex);
    }

    private static string Sha256Hex(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static void ConfigureApp(DirectoryInfo root)
    {
        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        File.WriteAllText(
            Path.Combine(root.FullName, "appsettings.json"),
            $$"""
            {
              "Culture": "pt-PT",
              "DatabaseSettings": {
                "DatabaseType": "Sqlite",
                "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                "UseCloud": false,
                "Module": "retail"
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        Assert.NotNull(AppComposition.Services);
    }

    private static void Cleanup(DirectoryInfo root)
    {
        SqliteConnection.ClearAllPools();
        AppLogging.CloseAndFlush();
        if (root.Exists)
        {
            try
            {
                root.Delete(recursive: true);
            }
            catch (IOException)
            {
            }
        }
    }
}
