using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Seeding;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Customers.Types;

public static class CustomerTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding customer types");
        if (database.CustomerTypes.Any())
        {
            logger.LogInformation("Customer types already present; skipping seed.");
            return true;
        }

        string seedFilePath = ModuleSeedFiles.Resolve(dbSettings, "customertypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Customer types seed file not found (module and default).");
            return false;
        }

        try
        {
            List<CustomerType>? customerTypes = JsonSerializer.Deserialize<List<CustomerType>>(File.ReadAllText(seedFilePath));
            if (customerTypes is null)
            {
                logger.LogError("Customer types seed file is empty.");
                return false;
            }
            database.CustomerTypes.AddRange(customerTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding customer types");
            return false;
        }
        
        logger.LogInformation("Customer types seeded successfully");
        return true;
    }
}
