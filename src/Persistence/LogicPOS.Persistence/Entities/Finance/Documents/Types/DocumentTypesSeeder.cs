using System.Text.Json;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Finance.Documents.Types;

public static class DocumentTypesSeeder
{
    public static bool SeedData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2, ILogger logger)
    {
        logger.LogInformation("Seeding document types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "documenttypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Document types seed file not found.");
            return false;
        }

        try
        {
            List<DocumentType>? documentTypes = JsonSerializer.Deserialize<List<DocumentType>>(File.ReadAllText(seedFilePath));
            if (documentTypes is null)
            {
                logger.LogError("Document types seed file is empty.");
                return false;
            }
            database.DocumentTypes.AddRange(documentTypes);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding document types");
            return false;
        }

        database.SaveChanges();
        logger.LogInformation("Document types seeded successfully");
        return true;
    }

    public static bool SeedNewData(LogicPOSDbContext database, DatabaseSettings dbSettings, string countryCode2,
        ILogger logger)
    {
        logger.LogInformation("Seeding new document types");
        string seedFilePath = Path.Combine(dbSettings.SeedPath!, countryCode2, "documenttypes.json");
        if (File.Exists(seedFilePath) == false)
        {
            logger.LogError("Document types seed file not found.");
            return false;
        }

        try
        {
            List<DocumentType>? seedDocumentTypes =
                JsonSerializer.Deserialize<List<DocumentType>>(File.ReadAllText(seedFilePath));
            if (seedDocumentTypes is null)
            {
                logger.LogError("Document types seed file is empty.");
                return false;
            }

            var existing = database.DocumentTypes
                .Select(x => new { x.Id, x.Acronym })
                .ToList();
            var existentAcronyms = new HashSet<string>(
                existing.Select(x => x.Acronym),
                StringComparer.OrdinalIgnoreCase);
            var existentIds = existing.Select(x => x.Id).ToHashSet();

            var collided = seedDocumentTypes
                .Where(x => existentAcronyms.Contains(x.Acronym) == false && existentIds.Contains(x.Id))
                .ToList();
            if (collided.Count > 0)
            {
                logger.LogWarning(
                    "Skipped document types whose id already exists under another acronym: {DocumentTypes}",
                    collided.Select(x => x.Acronym).ToArray());
            }

            var newDocumentTypes = seedDocumentTypes
                .Where(x => existentAcronyms.Contains(x.Acronym) == false && existentIds.Contains(x.Id) == false)
                .ToList();
            
            if (newDocumentTypes.Count == 0)
            {
                logger.LogInformation("No new document types detected");
                return true;
            }

            logger.LogInformation("Detected {NewDocumentTypesCount} new document types: {DocumentTypes}",
                newDocumentTypes.Count, newDocumentTypes.Select(x => x.Acronym).ToArray());

            database.DocumentTypes.AddRange(newDocumentTypes);
            database.SaveChanges();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while seeding new document types");
            return false;
        }

        logger.LogInformation("New document types seeded successfully");
        return true;
    }
}
