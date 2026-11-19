using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogicPOS.Persistence.Entities.Finance.Countries.PostalLocalities;

/// <summary>
/// Postal catalog read from <c>db_seeds/{country}/postallocalities.json</c> and
/// <c>postalstreets.json</c>. It is not copied into the database.
/// </summary>
public static class PostalCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly object Gate = new();
    private static readonly Dictionary<string, PostalFile> Cache = new(StringComparer.OrdinalIgnoreCase);

    public readonly record struct Entry(string ZipCode, string Street, string Locality, string City);

    public static IReadOnlyList<Entry> Take(string seedPath, string countryCode2, string? term, int take, out bool capped)
    {
        take = take <= 0 ? 25 : take;
        var file = Load(seedPath, countryCode2);
        var needle = (term ?? string.Empty).Trim();
        var found = new List<Entry>(Math.Min(take, 64));
        foreach (var entry in file.Entries)
        {
            if (needle.Length > 0 && Matches(entry, needle) == false)
            {
                continue;
            }

            if (found.Count == take)
            {
                capped = true;
                return found;
            }

            found.Add(entry);
        }

        capped = false;
        return found;
    }

    public static DateTime UpdatedAt(string seedPath, string countryCode2)
        => Load(seedPath, countryCode2).UpdatedAt;

    private static bool Matches(in Entry entry, string term)
        => entry.ZipCode.StartsWith(term, StringComparison.OrdinalIgnoreCase)
           || entry.Street.Contains(term, StringComparison.OrdinalIgnoreCase)
           || entry.Locality.Contains(term, StringComparison.OrdinalIgnoreCase)
           || entry.City.Contains(term, StringComparison.OrdinalIgnoreCase);

    private static PostalFile Load(string seedPath, string countryCode2)
    {
        var code = (countryCode2 ?? string.Empty).Trim().ToLowerInvariant();
        var folder = Path.Combine(string.IsNullOrWhiteSpace(seedPath) ? "db_seeds" : seedPath, code);
        lock (Gate)
        {
            if (Cache.TryGetValue(folder, out var cached))
            {
                return cached;
            }

            var loaded = Read(folder);
            Cache[folder] = loaded;
            return loaded;
        }
    }

    private static PostalFile Read(string folder)
    {
        var localitiesPath = Path.Combine(folder, "postallocalities.json");
        var streetsPath = Path.Combine(folder, "postalstreets.json");
        var updated = DateTime.MinValue;
        if (File.Exists(localitiesPath))
        {
            updated = File.GetLastWriteTime(localitiesPath);
        }

        if (File.Exists(streetsPath))
        {
            var streetsUpdated = File.GetLastWriteTime(streetsPath);
            if (streetsUpdated > updated)
            {
                updated = streetsUpdated;
            }
        }

        var shared = new Dictionary<string, string>(StringComparer.Ordinal);
        var entries = new List<Entry>(1024);
        var zips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in ReadRows(localitiesPath))
        {
            var zip = Share(shared, row.ZipCode?.Trim() ?? string.Empty);
            if (zip.Length == 0 || zips.Add(zip) == false)
            {
                continue;
            }

            var locality = Share(shared, row.Locality?.Trim() ?? string.Empty);
            var city = Share(shared, string.IsNullOrWhiteSpace(row.City) ? locality : row.City.Trim());
            entries.Add(new Entry(zip, string.Empty, locality, city));
        }

        var streets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in ReadRows(streetsPath))
        {
            var zip = Share(shared, row.ZipCode?.Trim() ?? string.Empty);
            var street = row.Street?.Trim() ?? string.Empty;
            if (zip.Length == 0 || street.Length == 0 || streets.Add(zip + "\n" + street) == false)
            {
                continue;
            }

            var locality = Share(shared, row.Locality?.Trim() ?? string.Empty);
            var city = Share(shared, string.IsNullOrWhiteSpace(row.City) ? locality : row.City.Trim());
            entries.Add(new Entry(zip, street, locality, city));
        }

        var array = entries.ToArray();
        Array.Sort(array, Compare);
        return new PostalFile(array, updated);
    }

    private static int Compare(Entry left, Entry right)
    {
        var leftName = left.Street.Length == 0 ? left.Locality : left.Street;
        var rightName = right.Street.Length == 0 ? right.Locality : right.Street;
        var compared = string.Compare(leftName, rightName, StringComparison.OrdinalIgnoreCase);
        if (compared != 0)
        {
            return compared;
        }

        compared = string.Compare(left.ZipCode, right.ZipCode, StringComparison.OrdinalIgnoreCase);
        if (compared != 0)
        {
            return compared;
        }

        compared = string.Compare(left.City, right.City, StringComparison.OrdinalIgnoreCase);
        if (compared != 0)
        {
            return compared;
        }

        return string.Compare(left.Locality, right.Locality, StringComparison.OrdinalIgnoreCase);
    }

    private static string Share(Dictionary<string, string> shared, string value)
    {
        if (value.Length == 0)
        {
            return string.Empty;
        }

        if (shared.TryGetValue(value, out var existing))
        {
            return existing;
        }

        shared[value] = value;
        return value;
    }

    private static List<PostalRow> ReadRows(string path)
    {
        if (File.Exists(path) == false)
        {
            return [];
        }

        var rows = JsonSerializer.Deserialize<List<PostalRow>>(File.ReadAllBytes(path), JsonOptions);
        return rows ?? [];
    }

    private sealed class PostalFile(Entry[] entries, DateTime updatedAt)
    {
        public Entry[] Entries { get; } = entries;
        public DateTime UpdatedAt { get; } = updatedAt;
    }

    private sealed class PostalRow
    {
        [JsonPropertyName("ZipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("Street")]
        public string? Street { get; set; }

        [JsonPropertyName("Locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("City")]
        public string? City { get; set; }
    }
}
