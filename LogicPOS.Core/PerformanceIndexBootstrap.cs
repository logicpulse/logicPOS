using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Core;

/// <summary>
/// Creates performance indexes idempotently on databases that may not have had
/// the EF migration applied (e.g. 1.5.x databases migrated to 1.6).
/// </summary>
public static class PerformanceIndexBootstrap
{
    private static readonly (string Table, string Name, string Columns)[] IndexDefinitions =
    {
        ("Articles", "IX_Articles_IsDeleted_Order", "IsDeleted, \"Order\""),
        ("Articles", "IX_Articles_Favorite", "Favorite"),
        ("ArticleFamilies", "IX_ArticleFamilies_IsDeleted_Order", "IsDeleted, \"Order\""),
        ("ArticleSubfamilies", "IX_ArticleSubfamilies_IsDeleted_Order", "IsDeleted, \"Order\""),
        ("Documents", "IX_Documents_CreatedAt", "CreatedAt"),
        ("Documents", "IX_Documents_IsDeleted_CreatedAt", "IsDeleted, CreatedAt"),
        ("Customers", "IX_Customers_IsDeleted", "IsDeleted"),
        ("Customers", "IX_Customers_FiscalNumber", "FiscalNumber")
    };

    /// <summary>
    /// Ensures performance indexes exist on the database.
    /// Safe to call on any database version - uses idempotent CREATE INDEX IF NOT EXISTS.
    /// </summary>
    public static void EnsureIndexes(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var logger = scope.ServiceProvider.GetService<ILogger<LogicPOSDbContext>>();

            var providerName = database.Database.ProviderName ?? string.Empty;
            var isSqlite = providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
            var isMySql = providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase)
                       || providerName.Contains("Pomelo", StringComparison.OrdinalIgnoreCase);
            var isSqlServer = providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

            foreach (var (table, name, columns) in IndexDefinitions)
            {
                try
                {
                    var sql = CreateIndexSql(table, name, columns, isSqlite, isMySql, isSqlServer);
                    if (string.IsNullOrEmpty(sql))
                    {
                        continue;
                    }

                    database.Database.ExecuteSqlRaw(sql);
                }
                catch (Exception ex)
                {
                    logger?.LogDebug(ex, "Index {IndexName} creation skipped (may already exist)", name);
                }
            }

            logger?.LogInformation("Performance indexes bootstrap completed");
        }
        catch (Exception ex)
        {
            var logger = services.GetService<ILogger<LogicPOSDbContext>>();
            logger?.LogWarning(ex, "Performance indexes bootstrap failed (non-fatal)");
        }
    }

    private static string CreateIndexSql(string table, string name, string columns, bool isSqlite, bool isMySql, bool isSqlServer)
    {
        if (isSqlite)
        {
            return $"CREATE INDEX IF NOT EXISTS \"{name}\" ON \"{table}\" ({columns})";
        }

        if (isMySql)
        {
            var mysqlColumns = columns.Replace("\"Order\"", "`Order`");
            return $@"
                SET @exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
                               WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{table}' AND INDEX_NAME = '{name}');
                SET @sql = IF(@exists = 0, 'CREATE INDEX `{name}` ON `{table}` ({mysqlColumns})', 'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;";
        }

        if (isSqlServer)
        {
            var sqlServerColumns = columns.Replace("\"Order\"", "[Order]");
            return $@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = '{name}' AND object_id = OBJECT_ID('{table}'))
                BEGIN
                    CREATE INDEX [{name}] ON [{table}] ({sqlServerColumns})
                END";
        }

        return string.Empty;
    }
}
