namespace LogicPOS.Core;

/// <summary>
/// SQLite always lives beside the installed exe (<see cref="AppContext.BaseDirectory"/>).
/// The installer must grant Users modify on that folder (Program Files is read-only otherwise).
/// </summary>
public static class SqliteDataPath
{
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

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredDataSource));
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
