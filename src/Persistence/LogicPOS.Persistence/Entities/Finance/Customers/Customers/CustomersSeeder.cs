using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Customers.Customers;

public static class CustomersSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2, ILogger logger)
    {
        logger.LogInformation("Seeding customers");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "customers.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Customers seed file not found.");
            return false;
        }

        try
        {
            List<Customer>? customers = JsonSerializer.Deserialize<List<Customer>>(File.ReadAllText(seedFilePath));
            if (customers is null)
            {
                logger.LogError("Customers seed file is empty.");
                return false;
            }
            database.Customers.AddRange(customers);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding customers");
            return false;
        }
        
        logger.LogInformation("Customers seeded successfully");
        return true;
    }
}
