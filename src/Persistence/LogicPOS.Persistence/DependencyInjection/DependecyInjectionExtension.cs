using LogicPOS.Application.Features.Finance.Customers;
using LogicPOS.Application.Features.System;
using LogicPOS.Application.Features.System.Licensing;
using LogicPOS.Persistence.Cloud;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using LogicPOS.Persistence.Seeding;
using LogicPOS.Persistence.Services;
using LogicPOS.Persistence.Services.DatabaseBackup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LogicPOS.Persistence.DependencyInjection;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
                                                    IConfiguration configuration)
    {
        DatabaseSettings databaseSettings = DatabaseSettings.GetFromConfiguration(configuration);

        if (databaseSettings.IsValid() == false)
        {
            Log.Fatal("Invalid Database settings: {Settings}",databaseSettings);
            throw new ArgumentException("Invalid database settings");   
        }
        
        services.AddSingleton(databaseSettings);
        services.AddScoped<TenantContext>();
        services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();
        services.AddSingleton<ICloudAccountLicenseReader, CloudAccountLicenseReader>();
        if (databaseSettings.UseCloud)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICloudAccountCatalog, SqlCloudAccountCatalog>();
        }
        else
        {
            services.AddSingleton<ICloudAccountCatalog, NullCloudAccountCatalog>();
        }

        services.AddScoped<AuditingInterceptor>();
        services.AddDatabase(databaseSettings);
        services.AddRepositories();
        services.AddTransient<IDatabaseBackupService, DatabaseBackupService>();
        services.AddScoped<IDatabaseMigrationsService, DatabaseMigrationsService>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<ICustomerOutstandingBalanceService, CustomerOutstandingBalanceService>();
        return services;
    }
}
