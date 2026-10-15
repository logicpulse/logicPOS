namespace LogicPOS.Core.Updates;

/// <summary>
/// Compares a published update with the version installed on disk.
/// Text after '+' (channel, git SHA) is not part of the release number, and a
/// 3-part version is the same release as that number with a trailing .0.
/// </summary>
public static class ApplicationVersionComparer
{
    public static bool IsNewer(string? packageVersion, string? installedVersion)
    {
        var package = ParseRelease(packageVersion);
        if (package is null)
        {
            return false;
        }

        var installed = ParseRelease(installedVersion);
        if (installed is null)
        {
            return true;
        }

        return package.CompareTo(installed) > 0;
    }

    public static string Display(string? version)
    {
        var parsed = ParseRelease(version);
        if (parsed is null)
        {
            return string.IsNullOrWhiteSpace(version) ? string.Empty : version.Trim();
        }

        if (parsed.Revision <= 0)
        {
            return $"{parsed.Major}.{parsed.Minor}.{parsed.Build}";
        }

        return parsed.ToString();
    }

    /// <summary>
    /// Returns a 4-part version, or null when <paramref name="version"/> has no leading number.
    /// Missing components are zero, so "1.6.33" and "1.6.33.0" compare equal.
    /// </summary>
    public static Version? ParseRelease(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return null;
        }

        var raw = version.Trim().TrimStart('\uFEFF');
        var plus = raw.IndexOf('+');
        if (plus >= 0)
        {
            raw = raw[..plus];
        }

        var dash = raw.IndexOf('-');
        if (dash >= 0)
        {
            raw = raw[..dash];
        }

        raw = raw.Trim().TrimStart('v', 'V');
        if (raw.Length == 0)
        {
            return null;
        }

        var parts = raw.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length is 0 or > 4)
        {
            return null;
        }

        var numbers = new int[4];
        for (var i = 0; i < parts.Length; i++)
        {
            if (int.TryParse(parts[i], out var value) == false || value < 0)
            {
                return null;
            }

            numbers[i] = value;
        }

        return new Version(numbers[0], numbers[1], numbers[2], numbers[3]);
    }
}
