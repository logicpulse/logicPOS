using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace LogicPOS.Core;

/// <summary>Reads and writes LogicPOS:Edition / LogicPOS:Hosting in appsettings.json.</summary>
public static class ProductEditionSettings
{
    public const string EditionKey = "LogicPOS:Edition";
    public const string HostingKey = "LogicPOS:Hosting";

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
    /// Infers edition for a 1.6 keep-config upgrade when LogicPOS:Edition is still missing.
    /// </summary>
    public static (ProductEdition Edition, ProductHosting Hosting) InferLegacy(
        IConfiguration configuration,
        string baseDirectory)
    {
        var useCloud = bool.TryParse(configuration["DatabaseSettings:UseCloud"], out var cloud) && cloud;
        var api = configuration["LogicPOS:Api:BaseAddress"] ?? string.Empty;
        if (useCloud
            && api.Contains("api.logic-pos.com", StringComparison.OrdinalIgnoreCase))
        {
            return (ProductEdition.Cloud, ProductHosting.Cloud);
        }

        var licencePath = Path.Combine(baseDirectory, "logicpos.licence");
        var legacyLicence = Path.Combine(baseDirectory, "logicpos.license");
        if (File.Exists(licencePath) || File.Exists(legacyLicence))
        {
            return (ProductEdition.Business, ProductHosting.Local);
        }

        if (useCloud)
            return (ProductEdition.Business, ProductHosting.RemoteApi);

        var apiFolder = Path.Combine(baseDirectory, "API");
        if (Directory.Exists(apiFolder)
            && File.Exists(Path.Combine(apiFolder, "logicposapi.exe")))
        {
            return (ProductEdition.Business, ProductHosting.LocalServer);
        }

        return (ProductEdition.Community, ProductHosting.Local);
    }

    public static void Write(
        string baseDirectory,
        ProductEdition edition,
        ProductHosting hosting,
        bool useCloud,
        string? apiBaseAddress = null,
        string? clientId = null)
    {
        var path = Path.Combine(baseDirectory, "appsettings.json");
        JsonNode root = File.Exists(path)
            ? JsonNode.Parse(File.ReadAllText(path)) ?? new JsonObject()
            : new JsonObject();

        var logicPos = root["LogicPOS"] as JsonObject ?? new JsonObject();
        root["LogicPOS"] = logicPos;
        logicPos["Edition"] = ProductEditionNames.ToConfig(edition);
        logicPos["Hosting"] = ProductHostingNames.ToConfig(hosting);

        var database = root["DatabaseSettings"] as JsonObject ?? new JsonObject();
        root["DatabaseSettings"] = database;
        database["UseCloud"] = useCloud;

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