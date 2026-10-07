using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.System.Audits;

public static class SystemAuditTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings,  ILogger logger)
    {
        logger.LogInformation("Seeding system audit types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, "systemaudittypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("System audit types seed file not found.");
            return false;
        }

        try
        {
            List<SystemAuditType>? auditTypes = JsonSerializer.Deserialize<List<SystemAuditType>>(File.ReadAllText(seedFilePath));
            if (auditTypes is null)
            {
                logger.LogError("System audit types seed file is empty.");
                return false;
            }
            database.SystemAuditTypes.AddRange(auditTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding system audit types");
            return false;
        }
        
        logger.LogInformation("System audit types seeded successfully");
        
        return true;
    }
}