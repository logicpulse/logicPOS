using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Users.Users;

public static class UsersSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        // 2.0: first interactive user is created by FirstUserSetupWizard (not JSON seeds).
        // Profiles / permissions still come from their own seeders.
        _ = database;
        _ = dbSettings;
        logger.LogInformation("Skipping users seed; first user is created at first run.");
        return true;
    }
}
