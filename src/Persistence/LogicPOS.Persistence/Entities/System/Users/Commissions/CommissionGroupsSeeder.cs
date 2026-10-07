using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Users.Commissions;

public static class CommissionGroupsSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding commission groups");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "commissiongroups.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Commission groups seed file not found.");
            return false;
        }

        try
        {
            List<CommissionGroup>? commissionGroups = JsonSerializer.Deserialize<List<CommissionGroup>>(File.ReadAllText(seedFilePath));
            if (commissionGroups is null)
            {
                logger.LogError("Commission groups seed file is empty.");
                return false;
            }
            database.CommissionGroups.AddRange(commissionGroups);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding commission groups");
            return false;
        }

        logger.LogInformation("Commission groups seeded successfully");
        return true;
    }
}
