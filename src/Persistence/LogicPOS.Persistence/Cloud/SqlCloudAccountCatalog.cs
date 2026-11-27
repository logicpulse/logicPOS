using System.Collections.Concurrent;
using System.Data.Common;
using LogicPOS.Persistence.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace LogicPOS.Persistence.Cloud;

public sealed class SqlCloudAccountCatalog : ICloudAccountCatalog
{
    private const string CacheKeyPrefix = "conn:tenant:";
    private static readonly TimeSpan SnapshotCacheDuration = TimeSpan.FromMinutes(10);

    /// <summary>
    /// How long a missing <c>Accounts.AvailableDevices</c> omission is remembered per catalog
    /// connection string before the next SELECT tries the column again (default 5 minutes).
    /// </summary>
    internal static readonly TimeSpan MissingColumnCacheTtl = TimeSpan.FromMinutes(5);

    /// <summary>
    /// UTC instant until which <c>AvailableDevices</c> is omitted for a given catalog connection string.
    /// Absence means the next query includes the column.
    /// </summary>
    private static readonly ConcurrentDictionary<string, DateTimeOffset> OmitAvailableDevicesUntil =
        new(StringComparer.Ordinal);

    private readonly DatabaseSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SqlCloudAccountCatalog> _logger;
    private readonly Func<string, DbConnection>? _connectionFactory;
    private readonly Func<DateTimeOffset> _utcNow;

    public SqlCloudAccountCatalog(
        DatabaseSettings settings,
        IMemoryCache cache,
        ILogger<SqlCloudAccountCatalog> logger)
        : this(settings, cache, logger, connectionFactory: null, utcNow: null)
    {
    }

    /// <summary>
    /// Test hook: non-SQL-Server connections (e.g. SQLite) and an injectable clock for omission TTL.
    /// </summary>
    internal SqlCloudAccountCatalog(
        DatabaseSettings settings,
        IMemoryCache cache,
        ILogger<SqlCloudAccountCatalog> logger,
        Func<string, DbConnection>? connectionFactory,
        Func<DateTimeOffset>? utcNow = null)
    {
        _settings = settings;
        _cache = cache;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
    }

    public CloudAccountSnapshot Find(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return CloudAccountSnapshot.NotFound;

        var key = CacheKeyPrefix + clientId.Trim();
        if (_cache.TryGetValue(key, out CloudAccountSnapshot? cached) && cached is not null)
            return cached;

        var snapshot = Query(clientId.Trim());
        _cache.Set(key, snapshot, SnapshotCacheDuration);
        return snapshot;
    }

    public void Invalidate(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return;

        _cache.Remove(CacheKeyPrefix + clientId.Trim());
    }

    public void SetOperationMode(string clientId, string mode)
    {
        var token = CloudOperationMode.NormalizeKnown(mode);
        if (string.IsNullOrWhiteSpace(clientId) || token is null)
            throw new InvalidOperationException("Modo de operação da conta inválido.");

        if (string.IsNullOrWhiteSpace(_settings.CloudConnectionString))
            throw new InvalidOperationException("A ligação ao catálogo cloud não está configurada.");

        var id = clientId.Trim();
        using var connection = OpenConnection(_settings.CloudConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Accounts
            SET PosOperationMode = @Mode
            WHERE state > 0
              AND AccountType_id = @AccountType
              AND ExpirationDate >= @Now
              AND (LOWER(Account_id) = LOWER(@Id) OR LOWER(Client_id) = LOWER(@Id))
            """;
        AddParameter(command, "@Mode", token);
        AddParameter(command, "@AccountType", CloudAccountTypes.LogicPosCloudService);
        AddParameter(command, "@Now", DateTime.Today);
        AddParameter(command, "@Id", id);
        var updated = command.ExecuteNonQuery();
        Invalidate(id);
        if (updated == 0)
            throw new InvalidOperationException($"A conta '{id}' não foi atualizada com o modo '{token}'.");
    }

    private static int ReadDeviceCount(object? value)
    {
        if (value is null || value is DBNull)
            return 0;

        if (value is int count)
            return count;

        return int.TryParse(value.ToString(), out var parsed) ? parsed : 0;
    }

    private bool ShouldIncludeAvailableDevices(string connectionString)
    {
        if (!OmitAvailableDevicesUntil.TryGetValue(connectionString, out var untilUtc))
            return true;

        if (_utcNow() >= untilUtc)
        {
            return OmitAvailableDevicesUntil.TryRemove(new KeyValuePair<string, DateTimeOffset>(connectionString, untilUtc))
                || !OmitAvailableDevicesUntil.ContainsKey(connectionString);
        }

        return false;
    }

    private void RememberMissingAvailableDevicesColumn(string connectionString)
    {
        OmitAvailableDevicesUntil[connectionString] = _utcNow() + MissingColumnCacheTtl;
    }

    private CloudAccountSnapshot Query(string clientId)
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudConnectionString))
            return CloudAccountSnapshot.NotFound;

        var connectionString = _settings.CloudConnectionString;
        var includeAvailableDevices = ShouldIncludeAvailableDevices(connectionString);

        try
        {
            return QueryOnce(clientId, connectionString, includeAvailableDevices);
        }
        catch (Exception ex) when (includeAvailableDevices && IsMissingAvailableDevicesColumn(ex))
        {
            RememberMissingAvailableDevicesColumn(connectionString);
            _logger.LogInformation(
                "Accounts.AvailableDevices is missing on the cloud catalog; loading accounts without it.");
            try
            {
                return QueryOnce(clientId, connectionString, includeAvailableDevices: false);
            }
            catch (Exception retryEx)
            {
                _logger.LogError(retryEx, "Failed to read cloud account for client id {ClientId}.", clientId);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read cloud account for client id {ClientId}.", clientId);
            throw;
        }
    }

    private CloudAccountSnapshot QueryOnce(string clientId, string connectionString, bool includeAvailableDevices)
    {
        using var connection = OpenConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelect(connection, includeAvailableDevices);
        AddParameter(command, "@AccountType", CloudAccountTypes.LogicPosCloudService);
        AddParameter(command, "@Now", DateTime.Today);
        AddParameter(command, "@Id", clientId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return CloudAccountSnapshot.NotFound;

        var accountConnectionString = reader["ConnectionString"]?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(accountConnectionString))
            return CloudAccountSnapshot.NotFound;

        var countryName = reader["CountryName"]?.ToString();
        var countryCode = CloudFiscalCountry.CodeFromCountryName(countryName);
        if (countryCode is null)
        {
            _logger.LogWarning(
                "Cloud account {ClientId} country '{CountryName}' is not PT, AO or MZ.",
                clientId,
                countryName);
        }

        var availableDevices = 0;
        if (includeAvailableDevices)
        {
            availableDevices = ReadDeviceCount(reader["AvailableDevices"]);
        }

        return new CloudAccountSnapshot
        {
            Found = true,
            ConnectionString = accountConnectionString,
            ModuleStocks = CloudAccountModuleFlag.FromDatabase(reader["ModuleStocks"]),
            ModuleOnlineInvoicing = CloudAccountModuleFlag.FromDatabase(reader["ModuleOnlineInvoicing"]),
            ModuleSms = CloudAccountModuleFlag.FromDatabase(reader["ModuleSMS"]),
            Demo = CloudAccountModuleFlag.FromDatabase(reader["Demo"]),
            CountryCode2 = countryCode,
            PosOperationMode = reader["PosOperationMode"]?.ToString(),
            Login = reader["Login"]?.ToString()?.Trim() ?? string.Empty,
            Password = reader["Password"]?.ToString() ?? string.Empty,
            AvailableDevices = availableDevices
        };
    }

    private DbConnection OpenConnection(string connectionString)
    {
        if (_connectionFactory is not null)
            return _connectionFactory(connectionString);

        return new SqlConnection(connectionString);
    }

    private static string BuildSelect(DbConnection connection, bool includeAvailableDevices)
    {
        var devices = includeAvailableDevices ? ", a.AvailableDevices" : string.Empty;
        if (connection is SqliteConnection)
        {
            return $"""
                SELECT a.ConnectionString, a.ModuleStocks, a.ModuleOnlineInvoicing, a.ModuleSMS, a.Demo, a.PosOperationMode, a.Login, a.[Password]{devices}, c.name AS CountryName
                FROM Accounts a
                LEFT JOIN Countries c ON c.id = a.idCountry
                WHERE a.state > 0
                  AND a.AccountType_id = @AccountType
                  AND a.ExpirationDate >= @Now
                  AND (LOWER(a.Account_id) = LOWER(@Id) OR LOWER(a.Client_id) = LOWER(@Id))
                  AND a.ConnectionString IS NOT NULL
                  AND LENGTH(TRIM(a.ConnectionString)) > 0
                LIMIT 1
                """;
        }

        return $"""
            SELECT TOP 1 a.ConnectionString, a.ModuleStocks, a.ModuleOnlineInvoicing, a.ModuleSMS, a.Demo, a.PosOperationMode, a.Login, a.[Password]{devices}, c.name AS CountryName
            FROM Accounts a
            LEFT JOIN Countries c ON c.id = a.idCountry
            WHERE a.state > 0
              AND a.AccountType_id = @AccountType
              AND a.ExpirationDate >= @Now
              AND (LOWER(a.Account_id) = LOWER(@Id) OR LOWER(a.Client_id) = LOWER(@Id))
              AND a.ConnectionString IS NOT NULL
              AND LEN(LTRIM(RTRIM(a.ConnectionString))) > 0
            """;
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    /// <summary>
    /// Strict missing-column detection: the error must name <c>AvailableDevices</c>.
    /// SQL Server 207, SQLite error 1 with «no such column», MySQL 1054 («Unknown column»).
    /// Another column's 207 / no such column / 1054 must not activate the omission path.
    /// </summary>
    internal static bool IsMissingAvailableDevicesColumn(Exception ex)
    {
        if (ex is SqlException sql)
            return sql.Errors.Cast<SqlError>().Any(error => error.Number == 207 &&
                error.Message.Contains("'AvailableDevices'", StringComparison.OrdinalIgnoreCase));

        if (ex is SqliteException sqlite)
            return sqlite.SqliteErrorCode == 1
                && (sqlite.Message.EndsWith("no such column: a.AvailableDevices'.", StringComparison.OrdinalIgnoreCase)
                    || sqlite.Message.EndsWith("no such column: AvailableDevices'.", StringComparison.OrdinalIgnoreCase));

        if (ex is MySqlException mysql)
            return mysql.Number == 1054
                && (mysql.Message.Contains("'AvailableDevices'", StringComparison.OrdinalIgnoreCase)
                    || mysql.Message.Contains("'a.AvailableDevices'", StringComparison.OrdinalIgnoreCase));

        return ex.InnerException is not null && IsMissingAvailableDevicesColumn(ex.InnerException);
    }

    /// <summary>Clears the per-connection-string AvailableDevices omission cache (tests only).</summary>
    internal static void ResetAvailableDevicesCacheForTests() => OmitAvailableDevicesUntil.Clear();
}
