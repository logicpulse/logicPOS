using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class PostgreSqlDependencyInjection
{
    private const string PostgreSqlMigrationsAssembly = "LogicPOS.Persistence.Migrators.PostgreSQL";

    public static void AddPostgreSql(this IServiceCollection services, DatabaseSettings databaseSettings)
    {
        // DateTime columns use unspecified Kind in the domain model (same as other engines).
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<LogicPOSDbContext>(
            options => options.UseNpgsql(
                databaseSettings.ConnectionString!,
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure()
                        .TranslateParameterizedCollectionsToConstants();
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    npgsqlOptions.MigrationsAssembly(PostgreSqlMigrationsAssembly);
                }),
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton);
    }
}
