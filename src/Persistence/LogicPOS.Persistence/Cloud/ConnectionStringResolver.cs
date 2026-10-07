using LogicPOS.Persistence.Database;
using Microsoft.Data.SqlClient;

namespace LogicPOS.Persistence.Cloud;

public sealed class ConnectionStringResolver : IConnectionStringResolver
{
    private readonly DatabaseSettings _settings;
    private readonly TenantContext _tenantContext;
    private readonly ICloudAccountCatalog _catalog;

    public ConnectionStringResolver(
        DatabaseSettings settings,
        TenantContext tenantContext,
        ICloudAccountCatalog catalog)
    {
        _settings = settings;
        _tenantContext = tenantContext;
        _catalog = catalog;
    }

    public string GetConnectionString()
    {
        if (TryGetConnectionString(out var connectionString) == false)
        {
            throw new InvalidOperationException(
                "Cloud mode without a client id uses DatabaseSettings.ConnectionString, and it is not set.");
        }

        return connectionString;
    }

    public bool TryGetConnectionString(out string connectionString)
    {
        if (_settings.UseCloud == false)
        {
            connectionString = _settings.ConnectionString;
            return true;
        }

        var clientId = _tenantContext.ClientId;
        if (string.IsNullOrWhiteSpace(clientId))
        {
            // GTK and any other caller that does not send a client id stay on the local database.
            connectionString = _settings.ConnectionString ?? string.Empty;
            return string.IsNullOrWhiteSpace(connectionString) == false;
        }

        var account = _catalog.Find(clientId);
        if (account.Found == false || string.IsNullOrWhiteSpace(account.ConnectionString))
        {
            throw new InvalidOperationException(
                $"Tenant '{clientId}' is not an active {CloudAccountTypes.LogicPosCloudService} account.");
        }

        connectionString = EnsureTrustServerCertificate(account.ConnectionString);
        return true;
    }

    public bool IsValidTenant(string? clientId)
    {
        if (_settings.UseCloud == false || string.IsNullOrWhiteSpace(clientId))
            return true;

        var account = _catalog.Find(clientId);
        return account.Found && string.IsNullOrWhiteSpace(account.ConnectionString) == false;
    }

    private static string EnsureTrustServerCertificate(string connectionString)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                TrustServerCertificate = true
            };
            return builder.ConnectionString;
        }
        catch (ArgumentException)
        {
            if (connectionString.Contains("TrustServerCertificate", StringComparison.OrdinalIgnoreCase))
                return connectionString;

            return connectionString.TrimEnd(';') + ";TrustServerCertificate=True";
        }
    }
}
