using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services;

/// <summary>
/// Creates performance indexes idempotently on databases that may not have had
/// the EF migration applied (e.g. 1.5.x databases migrated to 1.6).
/// </summary>
public static class PerformanceIndexBootstrap
{
    private static readonly (string Table, string Name, string[] Columns)[] IndexDefinitions =
    [
        ("Articles", "IX_Articles_IsDeleted_Order", ["IsDeleted", "Order"]),
        ("Articles", "IX_Articles_Favorite", ["Favorite"]),
        ("ArticleFamilies", "IX_ArticleFamilies_IsDeleted_Order", ["IsDeleted", "Order"]),
        ("ArticleSubfamilies", "IX_ArticleSubfamilies_IsDeleted_Order", ["IsDeleted", "Order"]),
        ("Documents", "IX_Documents_IsDeleted_CreatedAt", ["IsDeleted", "CreatedAt"]),
        ("Customers", "IX_Customers_IsDeleted", ["IsDeleted"])
    ];

    /// <summary>
    /// Ensures performance indexes exist on the database.
    /// Safe to call on any database version - creates indexes only if they do not exist.
    /// </summary>
    public static void EnsureIndexes(IServiceProvider services)
    {
        ILogger? logger = null;
        try
        {
            using var scope = services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(PerformanceIndexBootstrap));

            var providerName = database.Database.ProviderName ?? string.Empty;
            var isSqlite = providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
            var isMySql = providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase)
                       || providerName.Contains("Pomelo", StringComparison.OrdinalIgnoreCase);
            var isSqlServer = providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

            foreach (var (table, name, columns) in IndexDefinitions)
            {
                try
                {
                    if (isSqlite)
                    {
                        CreateIndexSqlite(database, table, name, columns);
                    }
                    else if (isMySql)
                    {
                        CreateIndexMySql(database, table, name, columns, logger);
                    }
                    else if (isSqlServer)
                    {
                        CreateIndexSqlServer(database, table, name, columns);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Index {IndexName} creation failed (non-fatal)", name);
                }
            }

            logger?.LogInformation("Performance indexes bootstrap completed");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Performance indexes bootstrap failed (non-fatal)");
        }
    }

    private static void CreateIndexSqlite(LogicPOSDbContext database, string table, string name, string[] columns)
    {
        var columnList = string.Join(", ", columns.Select(c => c == "Order" ? "\"Order\"" : c));
        var sql = $"CREATE INDEX IF NOT EXISTS \"{name}\" ON \"{table}\" ({columnList})";
        database.Database.ExecuteSqlRaw(sql);
    }

    private static void CreateIndexMySql(LogicPOSDbContext database, string table, string name, string[] columns, ILogger? logger)
    {
        try
        {
            var exists = database.Database
                .SqlQueryRaw<int>($"SELECT COUNT(*) AS Value FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{table}' AND INDEX_NAME = '{name}'")
                .AsEnumerable()
                .FirstOrDefault();

            if (exists > 0)
            {
                return;
            }

            var columnList = string.Join(", ", columns.Select(c => c == "Order" ? "`Order`" : $"`{c}`"));
            var sql = $"CREATE INDEX `{name}` ON `{table}` ({columnList})";
            database.Database.ExecuteSqlRaw(sql);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "MySQL index {IndexName} check/create failed", name);
        }
    }

    private static void CreateIndexSqlServer(LogicPOSDbContext database, string table, string name, string[] columns)
    {
        var columnList = string.Join(", ", columns.Select(c => c == "Order" ? "[Order]" : $"[{c}]"));
        var sql = $@"
            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes i
                INNER JOIN sys.objects o ON i.object_id = o.object_id
                WHERE i.name = N'{name}' AND o.name = N'{table}' AND o.type = 'U'
            )
            BEGIN
                CREATE INDEX [{name}] ON [{table}] ({columnList})
            END";
        database.Database.ExecuteSqlRaw(sql);
    }
}
