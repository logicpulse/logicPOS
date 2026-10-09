namespace LogicPOS.Core;

/// <summary>
/// Resolves the SQLite file path. Relative <c>Data Source=logicpos.db</c> cannot live under
/// Program Files when Users only have read+execute (SQLite Error 14). Prefer an existing file
/// beside the exe; otherwise a writable folder, then ProgramData.
/// </summary>
public static class SqliteDataPath
{
    public static string ProgramDataDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Logicpulse",
            "logicpos",
            "Data");

    public static string Resolve(string configuredDataSource)
    {
        if (string.IsNullOrWhiteSpace(configuredDataSource)
            || string.Equals(configuredDataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
            || configuredDataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return configuredDataSource;
        }

        if (Path.IsPathRooted(configuredDataSource))
        {
            return Path.GetFullPath(configuredDataSource);
        }

        var fileName = Path.GetFileName(configuredDataSource);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = "logicpos.db";
        }

        var besideExe = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredDataSource));
        if (File.Exists(besideExe))
        {
            return besideExe;
        }

        if (CanCreateFile(AppContext.BaseDirectory))
        {
            return besideExe;
        }

        var programData = Path.GetFullPath(Path.Combine(ProgramDataDirectory, fileName));
        Directory.CreateDirectory(ProgramDataDirectory);
        return programData;
    }

    public static bool CanCreateFile(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
            var probe = Path.Combine(directory, $".logicpos-write-{Guid.NewGuid():N}.tmp");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
