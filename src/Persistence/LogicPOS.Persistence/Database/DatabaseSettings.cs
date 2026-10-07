using Microsoft.Extensions.Configuration;
using Serilog;

namespace LogicPOS.Persistence.Database;

public record DatabaseSettings
{
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// When true, <see cref="CloudConnectionString"/> is the Accounts catalog and each request
    /// uses the tenant database stored on the LOGICPOS_CLOUDSERVICE account.
    /// </summary>
    public bool UseCloud { get; set; }

    /// <summary>Catalog database (Accounts) used only when <see cref="UseCloud"/> is true.</summary>
    public string? CloudConnectionString { get; set; }

    public string? SeedPath { get; set; }
    public bool UseSeed { get; set; } = true;
    public string? Module { get; set; } = null!;

    public bool IsValid()
    {
        if (UseCloud)
        {
            if (string.IsNullOrWhiteSpace(CloudConnectionString))
            {
                Log.Error("Cloud catalog connection string is not set");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                Log.Error("Local connection string is not set. Requests without a client id use it.");
                return false;
            }

            return true;
        }

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            Log.Error("Database connection string is not set");
            return false;
        }

        if (string.IsNullOrWhiteSpace(SeedPath))
        {
            Log.Error("Seed path is not set");       
            return false;
        }

        if (Directory.Exists(SeedPath) == false)
        {
            Log.Error("Seed directory does not exist");       
            return false;       
        }

        return Enum.IsDefined(typeof(DatabaseType), (int)DatabaseType);
    }

    public static readonly DatabaseSettings Default = new DatabaseSettings
    {
        DatabaseType = DatabaseType.Sqlite,
        ConnectionString = "Data Source=logicpos.db"
    };

    public static DatabaseSettings GetFromConfiguration(IConfiguration configuration)
    {
        var databaseSettings = new DatabaseSettings();
        configuration.GetSection("DatabaseSettings").Bind(databaseSettings);
        return databaseSettings;
    }
}