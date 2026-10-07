using System.IO;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class SqLiteDependencyInjection
{
    private const string SqliteMigrationsAssembly = "LogicPOS.Persistence.Migrators.SQLite";

    public static void AddSqlite(this IServiceCollection services, DatabaseSettings databaseSettings)
    {
        var connectionString = BuildSqliteConnectionString(databaseSettings.ConnectionString);

        services.AddDbContext<LogicPOSDbContext>(
            options =>
            {
                options.AddInterceptors(new SqlitePragmaConnectionInterceptor());
                options.UseSqlite(connectionString, sqliteOptions =>
                {
                    sqliteOptions.MigrationsAssembly(SqliteMigrationsAssembly);
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                });
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton);
    }

    /// <summary>
    ///     Caminho relativo no ficheiro de config depende do diretório de trabalho; fixamos em
    ///     <see cref="AppContext.BaseDirectory" />. <c>Cache=Shared</c> alivia bloqueios com várias
    ///     ligações ao mesmo ficheiro.
    /// </summary>
    private static string BuildSqliteConnectionString(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (ShouldResolveDataSourceToAppBaseDirectory(builder))
        {
            builder.DataSource = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, builder.DataSource));
        }

        if (builder.Cache is SqliteCacheMode.Default)
        {
            builder.Cache = SqliteCacheMode.Shared;
        }

        Log.Logger.Debug("SQLite: DataSource={DataSource}", builder.DataSource);

        return builder.ConnectionString;
    }

    private static bool ShouldResolveDataSourceToAppBaseDirectory(SqliteConnectionStringBuilder builder)
    {
        if (builder.Mode is SqliteOpenMode.Memory)
        {
            return false;
        }

        if (string.IsNullOrEmpty(builder.DataSource) ||
            string.Equals(builder.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (builder.DataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return Path.IsPathRooted(builder.DataSource) == false;
    }

}
