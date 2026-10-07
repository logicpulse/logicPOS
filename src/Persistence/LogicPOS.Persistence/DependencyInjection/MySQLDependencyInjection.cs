using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class MySqlDependencyInjection
{
    private const string MysqlMigrationsAssembly = "LogicPOS.Persistence.Migrators.MySQL";

    public static void AddMySql(this IServiceCollection services, DatabaseSettings databaseSettings)
    {
        services.AddDbContext<LogicPOSDbContext>(
            options =>
            {
                options.UseMySql(
                    databaseSettings.ConnectionString!,
                    ServerVersion.AutoDetect(databaseSettings.ConnectionString!),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnablePrimitiveCollectionsSupport()
                            .TranslateParameterizedCollectionsToConstants();
                        mySqlOptions.MigrationsAssembly(MysqlMigrationsAssembly);
                        mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    });
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton);
    }
}