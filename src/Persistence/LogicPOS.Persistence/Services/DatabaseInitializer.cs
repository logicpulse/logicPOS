using LogicPOS.Application.Features.System;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly LogicPOSDbContext _databaseContext;
    private readonly IDatabaseBackupService _backupService;
    private readonly IDatabaseMigrationsService _migrationsService;
    private readonly IDatabaseSeeder _databaseSeeder;
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(ILogger<DatabaseInitializer> logger, LogicPOSDbContext databaseContext,
        IDatabaseSeeder databaseSeeder, IDatabaseMigrationsService migrationsService,
        IDatabaseBackupService backupService, IConfiguration configuration)
    {
        _logger = logger;
        _databaseContext = databaseContext;
        _databaseSeeder = databaseSeeder;
        _migrationsService = migrationsService;
        _backupService = backupService;
        _configuration = configuration;
    }

    public bool Initialize()
    {
        _logger.LogInformation("Initializing database...");
        var databaseSettings = DatabaseSettings.GetFromConfiguration(_configuration);
        _logger.LogDebug("Database Settings: {DatabaseSettings}", databaseSettings);
        _backupService.EnsureSqliteRestore();

        bool databaseExistedBeforeMigrations = _databaseContext.DatabaseExists();

        if (databaseExistedBeforeMigrations == false)
        {
            _logger.LogWarning("Database doesn't exist and/or has no tables.");
        }

        _migrationsService.ApplyPendingMigrations();

        if (databaseExistedBeforeMigrations)
        {
            _logger.LogInformation("Database already existed before migrations");
            _databaseSeeder.ApplyNewSeed();
        }
        else
        {
            if (!_databaseContext.DatabaseExists())
            {
                _logger.LogCritical(
                    "Database doesn't exist even after migrations/initialization. Can't proceed");
                return false;
            }

            bool appliedRequiredSeed = _databaseSeeder.ApplyRequiredSeed();
            if (!appliedRequiredSeed)
            {
                _logger.LogCritical("Required seed wasn't applied. Can't proceed");
                return false;
            }

            if (databaseSettings.UseSeed)
            {
                _databaseSeeder.ApplyAdditionalSeed();
            }
            else
            {
                _databaseSeeder.ApplyRequiredUsersSeed();
            }
        }

        _databaseSeeder.EnsureCountrySpecificConfiguration();

        return true;
    }
}