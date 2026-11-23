using System.Security.Cryptography;
using System.Text;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.DataFixes;

/// <summary>
/// Clears legacy AT production preference-parameter Values that still match the former seed,
/// comparing SHA-256 (UTF-8) of the stored Value to known digests — never the plaintext.
/// Same EF Core path for SQLite, MySQL, SQL Server and PostgreSQL (SQLite has no native SHA).
/// </summary>
public static class FixClearAtProductionCredentialsR4
{
    public const string MigrationId = "20261125120000_FixClearAtProductionCredentialsR4";

    public const string PasswordToken = "SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD";
    public const string FiscalNumberToken = "SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER";

    public static readonly Guid PasswordId = Guid.Parse("f6248be8-a992-41fb-a403-65755eafc6f7");
    public static readonly Guid FiscalNumberId = Guid.Parse("a2c35299-c17d-42db-b4d1-a8c87f3a0ec0");

    /// <summary>SHA-256 hex (lowercase) of the former PT seed Value for <see cref="PasswordToken"/>.</summary>
    public const string DefaultPasswordValueSha256Hex =
        "21d46ef89b66aec09f8c0388b86f1b842de81636e19dcd63e24ecc5f60299d4e";

    /// <summary>SHA-256 hex (lowercase) of the former PT seed Value for <see cref="FiscalNumberToken"/>.</summary>
    public const string DefaultFiscalNumberValueSha256Hex =
        "fafc0575c483ef384b2a3387c1245fcf8bbbe2e7fea7b06abbb283cc7474c801";

    /// <summary>No-op SQL recorded in __EFMigrationsHistory; clearing runs in <see cref="Apply"/>.</summary>
    public const string HistoryMarkerSql = "SELECT 1;";

    public static string Sha256Hex(string? value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>
    /// Clears matching rows. Optional hash overrides are for tests (fictitious value + injectable digest).
    /// </summary>
    public static int Apply(
        LogicPOSDbContext database,
        string? passwordValueSha256Hex = null,
        string? fiscalNumberValueSha256Hex = null)
    {
        ArgumentNullException.ThrowIfNull(database);

        var passwordHash = (passwordValueSha256Hex ?? DefaultPasswordValueSha256Hex).Trim().ToLowerInvariant();
        var fiscalHash = (fiscalNumberValueSha256Hex ?? DefaultFiscalNumberValueSha256Hex).Trim().ToLowerInvariant();

        var cleared = 0;
        cleared += ClearIfHashMatches(database, PasswordId, PasswordToken, passwordHash);
        cleared += ClearIfHashMatches(database, FiscalNumberId, FiscalNumberToken, fiscalHash);

        if (cleared > 0)
        {
            database.SaveChanges();
        }

        return cleared;
    }

    private static int ClearIfHashMatches(
        LogicPOSDbContext database,
        Guid id,
        string token,
        string expectedSha256Hex)
    {
        var row = database.PreferenceParameters
            .FirstOrDefault(parameter => parameter.Id == id || parameter.Token == token);

        if (row is null)
        {
            return 0;
        }

        if (!string.Equals(row.Token, token, StringComparison.Ordinal))
        {
            return 0;
        }

        if (!string.Equals(Sha256Hex(row.Value), expectedSha256Hex, StringComparison.Ordinal))
        {
            return 0;
        }

        row.Value = string.Empty;
        return 1;
    }
}
