using LogicPOS.Persistence.Cloud;
using LogicPOS.Persistence.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class SqlServerDependencyInjection
{
    private const string SqlServerMigrationsAssembly = "LogicPOS.Persistence.Migrators.SQLServer";

    /// <summary>
    /// Never opened. Used only while an endpoint is constructed outside a tenant request.
    /// </summary>
    private const string UnresolvedTenantConnectionString =
        "Server=127.0.0.1,1;Database=__logicpos_cloud_no_tenant__;TrustServerCertificate=True;Connect Timeout=1";

    public static void AddSqlServer(this IServiceCollection services, DatabaseSettings databaseSettings)
    {
        services.AddDbContext<LogicPOSDbContext>(
            options => options.UseSqlServer(
                databaseSettings.ConnectionString!,
                sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure()
                                  .TranslateParameterizedCollectionsToConstants();
                    sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    sqlServerOptions.MigrationsAssembly(SqlServerMigrationsAssembly);
                }),
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton);
    }

    public static void AddCloudSqlServer(this IServiceCollection services)
    {
        services.AddDbContext<LogicPOSDbContext>(
            (serviceProvider, options) =>
            {
                var resolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
                // No client id (GTK, startup) uses the local connection. A client id uses the tenant database.
                var connectionString = resolver.TryGetConnectionString(out var resolvedConnection)
                    ? resolvedConnection
                    : UnresolvedTenantConnectionString;
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure()
                                      .TranslateParameterizedCollectionsToConstants();
                        sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                        sqlServerOptions.MigrationsAssembly(SqlServerMigrationsAssembly);
                    });
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);
    }
}
