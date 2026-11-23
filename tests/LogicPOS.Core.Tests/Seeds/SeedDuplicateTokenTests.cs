using System.Text.Json;

namespace LogicPOS.Core.Tests.Seeds;

public sealed class SeedDuplicateTokenTests
{
    [Fact]
    public void SeedJsonArrays_HaveNoDuplicateTokens()
    {
        var seedRoot = Path.Combine(SeedTestPaths.FindRepoRoot(), "db_seeds");
        Assert.True(Directory.Exists(seedRoot), $"Missing seed root: {seedRoot}");

        var failures = new List<string>();
        foreach (var path in Directory.EnumerateFiles(seedRoot, "*.json", SearchOption.AllDirectories).OrderBy(item => item, StringComparer.Ordinal))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                continue;
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

            if (tokens.Count == 0)
            {
                continue;
            }

            var duplicates = tokens
                .GroupBy(item => item, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => $"{group.Key} x{group.Count()}")
                .ToList();

            if (duplicates.Count > 0)
            {
                var relative = Path.GetRelativePath(seedRoot, path);
                failures.Add($"{relative}: {string.Join(", ", duplicates)}");
            }
        }

        Assert.True(failures.Count == 0, "Duplicate Token values in seed JSON:\n" + string.Join('\n', failures));
    }
}
