using LogicPOS.Application.Features.System;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LogicPOS.Persistence.Services;

public class DatabaseMigrationsService : IDatabaseMigrationsService
{
    private readonly ILogger<DatabaseMigrationsService> _logger;
    private readonly LogicPOSDbContext _database;


    public DatabaseMigrationsService(ILogger<DatabaseMigrationsService> logger, LogicPOSDbContext database)
    {
        _logger = logger;
        _database = database;
    }

    public int ApplyPendingMigrations()
    {
        var pendingMigrations = _database.Database.GetPendingMigrations().ToArray();

        if (pendingMigrations.Any() == false)
        {
            _logger.LogInformation("No pending migrations.");
            return 0;
        }

        _logger.LogInformation("Pending migrations: {PendingMigrations}", string.Join(", ", pendingMigrations));
        _logger.LogInformation("Applying pending migrations...");
        
        _database.Database.Migrate();
        
        _logger.LogInformation("Pending migrations applied successfully.");

        return pendingMigrations.Length;
    }
    
  
}