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
    /// <summary>
    /// Fixed whitelist of index definitions. DDL identifiers (table, index, column names)
    /// are built only from these constants, never from external input.
    /// </summary>
    private static readonly IndexDefinition[] IndexDefinitions =
    [
        new("Articles", "IX_Articles_IsDeleted_Order", ["IsDeleted", "Order"]),
        new("Articles", "IX_Articles_Favorite", ["Favorite"]),
        new("ArticleFamilies", "IX_ArticleFamilies_IsDeleted_Order", ["IsDeleted", "Order"]),
        new("ArticleSubfamilies", "IX_ArticleSubfamilies_IsDeleted_Order", ["IsDeleted", "Order"]),
        new("Documents", "IX_Documents_IsDeleted_CreatedAt", ["IsDeleted", "CreatedAt"]),
        new("Customers", "IX_Customers_IsDeleted", ["IsDeleted"])
    ];

    private static readonly HashSet<string> AllowedTables = new(
        IndexDefinitions.Select(d => d.Table),
        StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<string> AllowedIndexNames = new(
        IndexDefinitions.Select(d => d.Name),
        StringComparer.OrdinalIgnoreCase);

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
            var isPostgreSql = providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase)
                               || providerName.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase);

            foreach (var definition in IndexDefinitions)
            {
                try
                {
                    if (isSqlite)
                    {
                        CreateIndexSqlite(database, definition);
                    }
                    else if (isMySql)
                    {
                        CreateIndexMySql(database, definition, logger);
                    }
                    else if (isSqlServer)
                    {
                        CreateIndexSqlServer(database, definition);
                    }
                    else if (isPostgreSql)
                    {
                        CreateIndexPostgreSql(database, definition);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Index {IndexName} creation failed (non-fatal)", definition.Name);
                }
            }

            logger?.LogInformation("Performance indexes bootstrap completed");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Performance indexes bootstrap failed (non-fatal)");
        }
    }

    private static void CreateIndexSqlite(LogicPOSDbContext database, IndexDefinition definition)
    {
        ValidateDefinition(definition);
        var columnList = FormatColumns(definition.Columns, c => c == "Order" ? "\"Order\"" : $"\"{c}\"");
        var sql = $"CREATE INDEX IF NOT EXISTS \"{definition.Name}\" ON \"{definition.Table}\" ({columnList})";
        database.Database.ExecuteSqlRaw(sql);
    }

    private static void CreateIndexMySql(LogicPOSDbContext database, IndexDefinition definition, ILogger? logger)
    {
        ValidateDefinition(definition);
        try
        {
            var exists = database.Database
                .SqlQueryRaw<int>(
                    "SELECT COUNT(*) AS Value FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = {0} AND INDEX_NAME = {1}",
                    definition.Table,
                    definition.Name)
                .AsEnumerable()
                .FirstOrDefault();

            if (exists > 0)
            {
                return;
            }

            var columnList = FormatColumns(definition.Columns, c => c == "Order" ? "`Order`" : $"`{c}`");
            var sql = $"CREATE INDEX `{definition.Name}` ON `{definition.Table}` ({columnList})";
            database.Database.ExecuteSqlRaw(sql);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "MySQL index {IndexName} check/create failed", definition.Name);
        }
    }

    private static void CreateIndexSqlServer(LogicPOSDbContext database, IndexDefinition definition)
    {
        ValidateDefinition(definition);

        var existsQuery = database.Database
            .SqlQueryRaw<int>(
                @"SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM sys.indexes i
                    INNER JOIN sys.objects o ON i.object_id = o.object_id
                    WHERE i.name = {0} AND o.name = {1} AND o.type = 'U'
                ) THEN 1 ELSE 0 END AS Value",
                definition.Name,
                definition.Table)
            .AsEnumerable()
            .FirstOrDefault();

        if (existsQuery > 0)
        {
            return;
        }

        var columnList = FormatColumns(definition.Columns, c => c == "Order" ? "[Order]" : $"[{c}]");
        var sql = $"CREATE INDEX [{definition.Name}] ON [{definition.Table}] ({columnList})";
        database.Database.ExecuteSqlRaw(sql);
    }

    private static void CreateIndexPostgreSql(LogicPOSDbContext database, IndexDefinition definition)
    {
        ValidateDefinition(definition);
        var columnList = FormatColumns(definition.Columns, c => $"\"{c}\"");
        var sql = $"CREATE INDEX IF NOT EXISTS \"{definition.Name}\" ON \"{definition.Table}\" ({columnList})";
        database.Database.ExecuteSqlRaw(sql);
    }

    private static void ValidateDefinition(IndexDefinition definition)
    {
        if (!AllowedTables.Contains(definition.Table))
        {
            throw new InvalidOperationException($"Table '{definition.Table}' is not in the allowed whitelist.");
        }

        if (!AllowedIndexNames.Contains(definition.Name))
        {
            throw new InvalidOperationException($"Index '{definition.Name}' is not in the allowed whitelist.");
        }
    }

    private static string FormatColumns(string[] columns, Func<string, string> formatter)
    {
        return string.Join(", ", columns.Select(formatter));
    }

    private sealed record IndexDefinition(string Table, string Name, string[] Columns);
}
