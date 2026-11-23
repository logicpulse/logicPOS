using System.Text.Json;
using LogicPOS.Domain.Entities;

namespace LogicPOS.Core.Tests.Seeds;

/// <summary>
/// Validates that required seed JSON can be loaded in memory (deserialize + unique Token).
/// Full SQLite bootstrap may still fail on some CI images for unrelated native/runtime reasons.
/// </summary>
public sealed class RequiredSeedLoadTests
{
    private static readonly string[] CountryFolders = ["pt", "ao", "mz"];

    private static readonly string[] RequiredRootFiles =
    [
        "paymentmethods.json",
        "paymentconditions.json",
        "permissionitems.json",
        "permissiongroups.json",
        "countries.json",
        "currencies.json",
        "measurementunits.json",
        "sizeunits.json",
        "articleclasses.json",
        "discountgroups.json",
        "printertypes.json",
        "inputreaders.json",
        "poledisplays.json",
        "weighingmachines.json",
        "printers.json",
        "movementtypes.json",
        "systemaudittypes.json",
        "systemnotificationtypes.json",
        "warehouses.json",
        "warehouselocations.json"
    ];

    private static readonly string[] RequiredCountryFiles =
    [
        "preferenceparameters.json",
        "documenttypes.json",
        "vatrates.json",
        "vatexemptionreasons.json"
    ];

    [Fact]
    public void RequiredSeedFiles_DeserializeAndHaveUniqueTokens()
    {
        var seedRoot = Path.Combine(SeedTestPaths.FindRepoRoot(), "db_seeds");
        var failures = new List<string>();

        foreach (var country in CountryFolders)
        {
            foreach (var file in RequiredCountryFiles)
            {
                var path = Path.Combine(seedRoot, country, file);
                if (File.Exists(path) == false)
                {
                    failures.Add($"Missing required seed file: {country}/{file}");
                    continue;
                }

                if (file == "preferenceparameters.json")
                {
                    ValidateFile<PreferenceParameter>(path, failures, requireToken: true);
                }
                else if (file == "vatrates.json")
                {
                    ValidateFile<VatRate>(path, failures, requireToken: false);
                }
                else if (file == "vatexemptionreasons.json")
                {
                    ValidateFile<VatExemptionReason>(path, failures, requireToken: false);
                }
                else
                {
                    ValidateJsonTokens(path, failures);
                }
            }
        }

        foreach (var file in RequiredRootFiles)
        {
            var path = Path.Combine(seedRoot, file);
            if (File.Exists(path) == false)
            {
                failures.Add($"Missing required seed file: {file}");
                continue;
            }

            // Generic uniqueness check for Token when present.
            ValidateJsonTokens(path, failures);
        }

        Assert.True(failures.Count == 0, string.Join('\n', failures));
    }

    [Fact]
    public void PreferenceParameters_Pt_LoadsWithoutDuplicateIds()
    {
        var path = Path.Combine(SeedTestPaths.FindRepoRoot(), "db_seeds", "pt", "preferenceparameters.json");
        var items = JsonSerializer.Deserialize<List<PreferenceParameter>>(File.ReadAllText(path));
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.Equal(items.Count, items.Select(item => item.Token).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(items.Count, items.Select(item => item.Id).Distinct().Count());
        Assert.Contains(items, item => item.Token == "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS");
    }

    private static void ValidateFile<T>(string path, List<string> failures, bool requireToken)
    {
        if (File.Exists(path) == false)
        {
            failures.Add($"Missing: {path}");
            return;
        }

        try
        {
            var items = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path));
            if (items is null || items.Count == 0)
            {
                failures.Add($"Empty or null deserialize: {path}");
                return;
            }
        }
        catch (Exception ex)
        {
            failures.Add($"Deserialize failed for {path}: {ex.Message}");
            return;
        }

        if (requireToken)
        {
            ValidateJsonTokens(path, failures);
        }
    }

    private static void ValidateJsonTokens(string path, List<string> failures)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        var tokens = new List<string>();
        foreach (var item in document.RootElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            if (item.TryGetProperty("Token", out var tokenProperty) == false)
            {
                continue;
            }

            var token = tokenProperty.GetString();
            if (string.IsNullOrWhiteSpace(token) == false)
            {
                tokens.Add(token);
            }
        }

        var duplicates = tokens
            .GroupBy(item => item, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicates.Count > 0)
        {
            failures.Add($"{Path.GetFileName(path)} duplicate Token(s): {string.Join(", ", duplicates)}");
        }
    }
}
