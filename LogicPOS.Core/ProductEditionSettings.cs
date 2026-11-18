using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace LogicPOS.Core;

/// <summary>Reads and writes LogicPOS:Edition / LogicPOS:Hosting in appsettings.json.</summary>
public static class ProductEditionSettings
{
    public const string EditionKey = "LogicPOS:Edition";
    public const string HostingKey = "LogicPOS:Hosting";
    public const string SoftwareKeyConfig = "LogicPOS:SoftwareKey";

    public static bool HasEdition(IConfiguration configuration)
        => string.IsNullOrWhiteSpace(configuration[EditionKey]) == false;

    public static bool TryGet(
        IConfiguration configuration,
        out ProductEdition edition,
        out ProductHosting hosting)
    {
        hosting = ProductHosting.Local;
        if (ProductEditionNames.TryParse(configuration[EditionKey], out edition) == false)
            return false;

        _ = ProductHostingNames.TryParse(configuration[HostingKey], out hosting);
        return true;
    }

    /// <summary>
    /// Upgrades of 1.4 / 1.5 / 1.6 leave Edition empty so the choice page appears.
    /// Never invent Business or Cloud from an old licence file or API folder.
    /// </summary>
    public static (ProductEdition Edition, ProductHosting Hosting) InferLegacy(
        IConfiguration configuration,
        string baseDirectory)
    {
        _ = configuration;
        _ = baseDirectory;
        return (ProductEdition.Community, ProductHosting.Local);
    }

    public static void Write(
        string baseDirectory,
        ProductEdition edition,
        ProductHosting hosting,
        bool useCloud,
        string? apiBaseAddress = null,
        string? clientId = null,
        string? softwareKey = null,
        CloudClientSurface? cloudSurface = null)
    {
        var path = Path.Combine(baseDirectory, "appsettings.json");
        JsonNode root = File.Exists(path)
            ? JsonNode.Parse(File.ReadAllText(path)) ?? new JsonObject()
            : new JsonObject();

        var logicPos = root["LogicPOS"] as JsonObject ?? new JsonObject();
        root["LogicPOS"] = logicPos;
        logicPos["Edition"] = ProductEditionNames.ToConfig(edition);
        logicPos["Hosting"] = ProductHostingNames.ToConfig(hosting);

        if (string.IsNullOrWhiteSpace(softwareKey) == false)
            logicPos["SoftwareKey"] = softwareKey.Trim();
        else if (edition != ProductEdition.Business)
            logicPos.Remove("SoftwareKey");

        if (edition == ProductEdition.Cloud && cloudSurface is not null)
            logicPos["CloudSurface"] = CloudClientSurfaceNames.ToConfig(cloudSurface.Value);
        else
            logicPos.Remove("CloudSurface");

        var database = root["DatabaseSettings"] as JsonObject ?? new JsonObject();
        root["DatabaseSettings"] = database;
        database["UseCloud"] = useCloud;

        // Community / local Business: first-run edition write must not leave ConnectionString empty
        // (MSI ships UseCloud=true without a connection string; flipping to local would soft-lock login).
        if (useCloud == false)
        {
            if (database["ConnectionString"] is null
                || string.IsNullOrWhiteSpace(database["ConnectionString"]?.GetValue<string>()))
            {
                database["ConnectionString"] = "Data Source=logicpos.db";
            }

            if (database["DatabaseType"] is null
                || string.IsNullOrWhiteSpace(database["DatabaseType"]?.GetValue<string>()))
            {
                database["DatabaseType"] = "Sqlite";
            }

            if (database["SeedPath"] is null
                || string.IsNullOrWhiteSpace(database["SeedPath"]?.GetValue<string>()))
            {
                database["SeedPath"] = "db_seeds";
            }

            if (database["UseSeed"] is null)
            {
                database["UseSeed"] = false;
            }

            if (database["Module"] is null
                || string.IsNullOrWhiteSpace(database["Module"]?.GetValue<string>()))
            {
                database["Module"] = "default";
            }
        }

        if (string.IsNullOrWhiteSpace(apiBaseAddress) == false || string.IsNullOrWhiteSpace(clientId) == false)
        {
            var api = logicPos["Api"] as JsonObject ?? new JsonObject();
            logicPos["Api"] = api;
            if (string.IsNullOrWhiteSpace(apiBaseAddress) == false)
                api["BaseAddress"] = apiBaseAddress;
            if (clientId is not null)
                api["ClientId"] = clientId;
        }

        var json = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}