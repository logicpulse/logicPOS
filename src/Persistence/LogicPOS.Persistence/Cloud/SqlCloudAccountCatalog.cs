using LogicPOS.Persistence.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Cloud;

public sealed class SqlCloudAccountCatalog : ICloudAccountCatalog
{
    private const string CacheKeyPrefix = "conn:tenant:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly DatabaseSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SqlCloudAccountCatalog> _logger;

    public SqlCloudAccountCatalog(
        DatabaseSettings settings,
        IMemoryCache cache,
        ILogger<SqlCloudAccountCatalog> logger)
    {
        _settings = settings;
        _cache = cache;
        _logger = logger;
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

    private CloudAccountSnapshot Query(string clientId)
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudConnectionString))
            return CloudAccountSnapshot.NotFound;

        try
        {
            using var connection = new SqlConnection(_settings.CloudConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT TOP 1 a.ConnectionString, a.ModuleStocks, a.ModuleOnlineInvoicing, a.ModuleSMS, a.PosOperationMode, c.name AS CountryName
                FROM Accounts a
                LEFT JOIN Countries c ON c.id = a.idCountry
                WHERE a.state > 0
                  AND a.AccountType_id = @AccountType
                  AND a.ExpirationDate >= @Now
                  AND (LOWER(a.Account_id) = LOWER(@Id) OR LOWER(a.Client_id) = LOWER(@Id))
                  AND a.ConnectionString IS NOT NULL
                  AND LEN(LTRIM(RTRIM(a.ConnectionString))) > 0
                """;
            command.Parameters.Add(new SqlParameter("@AccountType", CloudAccountTypes.LogicPosCloudService));
            command.Parameters.Add(new SqlParameter("@Now", DateTime.Today));
            command.Parameters.Add(new SqlParameter("@Id", clientId));

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return CloudAccountSnapshot.NotFound;

            var connectionString = reader["ConnectionString"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(connectionString))
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

            return new CloudAccountSnapshot
            {
                Found = true,
                ConnectionString = connectionString,
                ModuleStocks = CloudAccountModuleFlag.FromDatabase(reader["ModuleStocks"]),
                ModuleOnlineInvoicing = CloudAccountModuleFlag.FromDatabase(reader["ModuleOnlineInvoicing"]),
                ModuleSms = CloudAccountModuleFlag.FromDatabase(reader["ModuleSMS"]),
                CountryCode2 = countryCode,
                PosOperationMode = reader["PosOperationMode"]?.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read cloud account for client id {ClientId}.", clientId);
            return CloudAccountSnapshot.NotFound;
        }
    }
}
