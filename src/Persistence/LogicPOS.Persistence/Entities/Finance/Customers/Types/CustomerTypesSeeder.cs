using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Customers.Types;

public static class CustomerTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, ILogger logger)
    {
        logger.LogInformation("Seeding customer types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!,"modules",dbSettings.Module!, "customertypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Customer types seed file not found.");
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
