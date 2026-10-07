using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Services.DatabaseBackup;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Persistence.DependencyInjection;

internal static class DatabaseDependencyInjectionExtension
{
    public static void AddDatabase(this IServiceCollection services,
                                   DatabaseSettings databaseSettings)
    {
        if (databaseSettings.UseCloud)
        {
            services.AddCloudSqlServer();
        }
        else
        {
            switch (databaseSettings.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    services.AddSqlServer(databaseSettings);
                    break;
                case DatabaseType.MySql:
                    services.AddMySql(databaseSettings);
                    break;
                case DatabaseType.Sqlite:
                    services.AddSqlite(databaseSettings);
                    break;
            }

            services.AddHostedService<ScheduledBackupService>();
        }
    }
}
