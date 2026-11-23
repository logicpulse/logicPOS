using System.Text.Json;

namespace LogicPOS.Core.Tests.Fiscal;

/// <summary>
/// Ensures Lei PT company fiscal-mark preference Ids do not collide with seeded Ids
/// (notably SYSTEM_CURRENCY_OID), matching EnsureLeiPtCompanyFiscalMarksAsync.
/// </summary>
public sealed class LeiPtFiscalMarkGuidTests
{
    // Keep in sync with BackOfficeListingService.EnsureLeiPtCompanyFiscalMarksAsync.
    private static readonly Guid[] LeiPtFiscalMarkIds =
    [
        Guid.Parse("c3d4e5f6-a7b8-4901-c234-56789abcde01"),
        Guid.Parse("d4e5f6a7-b8c9-4012-d345-6789abcdef12")
    ];

    private static readonly Guid SystemCurrencyOid = Guid.Parse("b2c3d4e5-f6a7-4890-b123-456789abcdef");

    [Fact]
    public void LeiPt_fiscal_mark_ids_do_not_collide_with_seed_preference_ids()
    {
        var seedIds = LoadSeedPreferenceIds();
        Assert.Contains(SystemCurrencyOid, seedIds);

        foreach (var id in LeiPtFiscalMarkIds)
        {
            Assert.NotEqual(SystemCurrencyOid, id);
            Assert.DoesNotContain(id, seedIds);
        }
    }

    private static HashSet<Guid> LoadSeedPreferenceIds()
    {
        var root = FindRepoRoot();
        var ids = new HashSet<Guid>();
        foreach (var culture in new[] { "pt", "ao", "mz" })
        {
            var path = Path.Combine(root, "db_seeds", culture, "preferenceparameters.json");
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("Id", out var idProperty)
                    && Guid.TryParse(idProperty.GetString(), out var id))
                {
                    ids.Add(id);
                }
            }
        }

        return ids;
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "LogicPOS.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Repository root not found.");
    }
}
