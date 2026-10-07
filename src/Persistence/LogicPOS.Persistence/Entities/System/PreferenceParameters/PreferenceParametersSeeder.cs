using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LogicPOS.Persistence.Entities.System.PreferenceParameters;

public static class PreferenceParametersSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2,
        ILogger logger)
    {
        logger.LogInformation("Seeding preference parameters");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "preferenceparameters.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Preference parameters seed file not found.");
            return false;
        }

        try
        {
            List<PreferenceParameter>? parameters =
                JsonSerializer.Deserialize<List<PreferenceParameter>>(File.ReadAllText(seedFilePath));
            if (parameters is null)
            {
                logger.LogError("Preference parameters seed file is empty.");
                return false;
            }

            database.PreferenceParameters.AddRange(parameters);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding preference parameters");
            return false;
        }

        logger.LogInformation("Preference parameters seeded successfully");

        return true;
    }

    public static bool SeedNewData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2,
        ILogger logger)
    {
        logger.LogInformation("Seeding new preference parameters");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "preferenceparameters.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Preference parameters seed file not found.");
            return false;
        }

        try
        {
            List<PreferenceParameter>? seedParameters =
                JsonSerializer.Deserialize<List<PreferenceParameter>>(File.ReadAllText(seedFilePath));

            if (seedParameters is null)
            {
                logger.LogError("Preference parameters seed file is empty.");
                return false;
            }

            string[] existentTokens = database.PreferenceParameters.Select(x => x.Token).ToArray();

            var newParameters = seedParameters.Where(x => !existentTokens.Contains(x.Token)).ToList();

            if (newParameters.Count == 0)
            {
                logger.LogInformation("No new preference parameters detected");
                return true;
            }

            logger.LogInformation("Detected {NewParametersCount} new preference parameters: {Parameters}",
                newParameters.Count, newParameters.Select(x => x.Token).ToArray());
            
            database.PreferenceParameters.AddRange(newParameters);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding new preference parameters");
            return false;
        }

        logger.LogInformation("New preference parameters seeded successfully");

        return true;
    }
}