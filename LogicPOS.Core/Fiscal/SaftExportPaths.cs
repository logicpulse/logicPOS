using System.Globalization;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Shared helpers for resolving the Avalonia SAF-T destination folder and file name.
/// </summary>
public static class SaftExportPaths
{
    public const string PreferenceToken = "PATH_SAFTPT";

    public static string BuildFileName(string? sourcePath = null)
    {
        var stamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var fromSource = string.IsNullOrWhiteSpace(sourcePath) ? null : Path.GetFileName(sourcePath);
        if (string.IsNullOrWhiteSpace(fromSource) == false
            && fromSource.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            && fromSource.Contains("saft", StringComparison.OrdinalIgnoreCase))
        {
            return fromSource;
        }

        return $"saft_pt_1.04_01_{stamp}.xml";
    }

    public static bool IsWritableDirectory(string? folder)
    {
        if (string.IsNullOrWhiteSpace(folder) || Directory.Exists(folder) == false)
        {
            return false;
        }

        try
        {
            var probe = Path.Combine(folder, $".logicpos_saft_write_{Guid.NewGuid():N}.tmp");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Copies <paramref name="sourcePath"/> into <paramref name="folder"/> when the folder is writable.
    /// </summary>
    public static bool TrySaveToFolder(string sourcePath, string? folder, out string destinationPath)
    {
        destinationPath = string.Empty;
        if (IsWritableDirectory(folder) == false)
        {
            return false;
        }

        try
        {
            destinationPath = Path.Combine(folder!.Trim(), BuildFileName(sourcePath));
            File.Copy(sourcePath, destinationPath, overwrite: true);
            return true;
        }
        catch
        {
            destinationPath = string.Empty;
            return false;
        }
    }
}
