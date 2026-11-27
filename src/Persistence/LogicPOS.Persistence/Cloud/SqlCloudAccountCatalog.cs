using System.Collections.Concurrent;
using System.Data.Common;
using LogicPOS.Persistence.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Cloud;

public sealed class SqlCloudAccountCatalog : ICloudAccountCatalog
{
    private const string CacheKeyPrefix = "conn:tenant:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Whether <c>Accounts.AvailableDevices</c> can be selected for a given cloud catalog connection string.
    /// Missing column is remembered so later calls skip it without a schema probe.
    /// </summary>
    private static readonly ConcurrentDictionary<string, bool> AvailableDevicesByConnection =
        new(StringComparer.Ordinal);

    private readonly DatabaseSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SqlCloudAccountCatalog> _logger;
    private readonly Func<string, DbConnection>? _connectionFactory;

    public SqlCloudAccountCatalog(
        DatabaseSettings settings,
        IMemoryCache cache,
        ILogger<SqlCloudAccountCatalog> logger)
        : this(settings, cache, logger, connectionFactory: null)
    {
    }

    /// <summary>Test hook: supply a non-SQL-Server connection (e.g. SQLite) without changing production DI.</summary>
    internal SqlCloudAccountCatalog(
        DatabaseSettings settings,
        IMemoryCache cache,
        ILogger<SqlCloudAccountCatalog> logger,
        Func<string, DbConnection>? connectionFactory)
    {
        _settings = settings;
        _cache = cache;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public CloudAccountSnapshot Find(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return CloudAccountSnapshot.NotFound;

        var key = CacheKeyPrefix + clientId.Trim();
        if (_cache.TryGetValue(key, out CloudAccountSnapshot? cached) && cached is not null)
            return cached;

        var snapshot = Query(clientId.Trim());
        _cache.Set(key, snapshot, CacheDuration);
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

    private CloudAccountSnapshot Query(string clientId)
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudConnectionString))
            return CloudAccountSnapshot.NotFound;

        var connectionString = _settings.CloudConnectionString;
        var includeAvailableDevices = AvailableDevicesByConnection.GetOrAdd(connectionString, _ => true);

        try
        {
            return QueryOnce(clientId, connectionString, includeAvailableDevices);
        }
        catch (Exception ex) when (includeAvailableDevices && IsMissingAvailableDevicesColumn(ex))
        {
            AvailableDevicesByConnection[connectionString] = false;
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
            availableDevices = ReadDeviceCount(reader["AvailableDevices"]);

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
    /// Prefer provider error numbers: SQL Server 207 (invalid column name), SQLite error code 1 with
    /// a missing-column message. Message text alone is not enough for SQL Server.
    /// </summary>
    internal static bool IsMissingAvailableDevicesColumn(Exception ex)
    {
        if (ex is SqlException sql)
            return sql.Errors.Cast<SqlError>().Any(error => error.Number == 207 &&
                error.Message.Contains("AvailableDevices", StringComparison.OrdinalIgnoreCase));

        if (ex is SqliteException sqlite)
            return sqlite.SqliteErrorCode == 1 &&
                   (sqlite.Message.Contains("no such column", StringComparison.OrdinalIgnoreCase)
                    || sqlite.Message.Contains("AvailableDevices", StringComparison.OrdinalIgnoreCase));

        return ex.InnerException is not null && IsMissingAvailableDevicesColumn(ex.InnerException);
    }

    /// <summary>Clears the per-connection-string AvailableDevices cache (tests only).</summary>
    internal static void ResetAvailableDevicesCacheForTests() => AvailableDevicesByConnection.Clear();
}
