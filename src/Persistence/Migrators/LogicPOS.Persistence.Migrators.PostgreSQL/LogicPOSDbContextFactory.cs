using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LogicPOS.Persistence.Migrators.PostgreSQL;

/// <summary>
/// Design-time factory used by <c>dotnet ef migrations</c> for the PostgreSQL migrator.
/// </summary>
public sealed class LogicPOSDbContextFactory : IDesignTimeDbContextFactory<LogicPOSDbContext>
{
    public LogicPOSDbContext CreateDbContext(string[] args)
    {
        // Must match runtime DI so the migration snapshot aligns with Migrate().
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var optionsBuilder = new DbContextOptionsBuilder<LogicPOSDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=127.0.0.1;Port=5432;Database=logicpos;Username=postgres;Password=postgres",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("LogicPOS.Persistence.Migrators.PostgreSQL");
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            });

        var auditing = new AuditingInterceptor(new DesignTimeAuditingInformationService());
        return new LogicPOSDbContext(optionsBuilder.Options, auditing);
    }

    private sealed class DesignTimeAuditingInformationService : IAuditingInformationService
    {
        public Guid? GetUserId() => null;
        public Guid? GetTerminalId() => null;
    }
}
