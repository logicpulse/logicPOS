using System.Text.Json;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Finance.Postal;

internal static class PortugalGeoApiPostalParser
{
    public static IReadOnlyList<PostalLocalityMatch> Parse(string json, int take)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        if (root.TryGetProperty("CP", out _))
        {
            var match = ParseComplete(root);
            return match is null ? [] : [match];
        }

        if (root.TryGetProperty("CP4", out var cp4Prop))
        {
            return ParsePrefix(root, cp4Prop.GetString() ?? string.Empty, take);
        }

        return [];
    }

    private static PostalLocalityMatch? ParseComplete(JsonElement root)
    {
        var zip = ReadString(root, "CP");
        if (zip.Length == 0)
        {
            return null;
        }

        var locality = ReadString(root, "Localidade");
        var city = ReadString(root, "Concelho");
        if (city.Length == 0)
        {
            city = locality;
        }

        var street = ReadStreet(root);
        return new PostalLocalityMatch
        {
            ZipCode = zip,
            Locality = locality,
            City = city,
            Street = street
        };
    }

    private static IReadOnlyList<PostalLocalityMatch> ParsePrefix(JsonElement root, string cp4, int take)
    {
        take = take <= 0 ? 25 : Math.Min(take, 50);
        var locality = ReadString(root, "Localidade");
        var city = ReadString(root, "Concelho");
        if (city.Length == 0)
        {
            city = locality;
        }

        if (root.TryGetProperty("CP3", out var cp3List) && cp3List.ValueKind == JsonValueKind.Array)
        {
            var matches = new List<PostalLocalityMatch>(Math.Min(take, cp3List.GetArrayLength()));
            foreach (var item in cp3List.EnumerateArray())
            {
                if (matches.Count == take)
                {
                    break;
                }

                var cp3 = item.GetString()?.Trim() ?? string.Empty;
                if (cp3.Length == 0)
                {
                    continue;
                }

                matches.Add(new PostalLocalityMatch
                {
                    ZipCode = cp4 + "-" + cp3,
                    Locality = locality,
                    City = city
                });
            }

            if (matches.Count > 0)
            {
                return matches;
            }
        }

        if (cp4.Length == 0)
        {
            return [];
        }

        return
        [
            new PostalLocalityMatch
            {
                ZipCode = cp4,
                Locality = locality,
                City = city
            }
        ];
    }

    private static string ReadStreet(JsonElement root)
    {
        if (root.TryGetProperty("partes", out var partes) && partes.ValueKind == JsonValueKind.Array)
        {
            foreach (var parte in partes.EnumerateArray())
            {
                var artery = ReadPropertyLoose(parte, "Artéria", "Arteria");
                if (artery.Length > 0)
                {
                    return artery;
                }
            }
        }

        if (root.TryGetProperty("ruas", out var ruas) && ruas.ValueKind == JsonValueKind.Array)
        {
            foreach (var rua in ruas.EnumerateArray())
            {
                var name = rua.GetString()?.Trim() ?? string.Empty;
                if (name.Length > 0)
                {
                    return name;
                }
            }
        }

        return string.Empty;
    }

    private static string ReadString(JsonElement root, string name)
        => root.TryGetProperty(name, out var value) ? value.GetString()?.Trim() ?? string.Empty : string.Empty;

    private static string ReadPropertyLoose(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (element.TryGetProperty(name, out var value))
            {
                var text = value.GetString()?.Trim() ?? string.Empty;
                if (text.Length > 0)
                {
                    return text;
                }
            }
        }

        return string.Empty;
    }
}