using System.Data;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Countries.PostalLocalities;

public static class PostalLocalitiesSeeder
{
    /// <summary>
    /// The postal catalog stays in the JSON files. Rows already copied into
    /// <c>PostalLocalities</c> are removed so the database file can shrink.
    /// </summary>
    public static bool SeedData(
        LogicPOSDbContext database,
        DatabaseSettings dbSettings,
        string countryCode2,
        ILogger logger)
    {
        try
        {
            if (database.PostalLocalities.Any())
            {
                database.Database.SetCommandTimeout(180);
                var removed = database.PostalLocalities.ExecuteDelete();
                logger.LogInformation(
                    "Removed {Count} postal rows for {Country}. Catalog stays in {SeedPath}.",
                    removed,
                    countryCode2,
                    dbSettings.SeedPath);
            }

            CompactSqliteIfBloated(database, logger);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while releasing copied postal rows");
            return false;
        }
    }

    private static void CompactSqliteIfBloated(LogicPOSDbContext database, ILogger logger)
    {
        var provider = database.Database.ProviderName ?? string.Empty;
        if (provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == false)
        {
            return;
        }

        var connection = database.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var probe = connection.CreateCommand();
            probe.CommandText = "PRAGMA freelist_count;";
            var free = Convert.ToInt64(probe.ExecuteScalar());
            if (free < 2000)
            {
                return;
            }

            logger.LogInformation("Compacting SQLite database ({FreePages} free pages).", free);
            using var vacuum = connection.CreateCommand();
            vacuum.CommandTimeout = 600;
            vacuum.CommandText = "VACUUM;";
            vacuum.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "SQLite database was not compacted. It will be retried on the next start.");
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}
