namespace LogicPOS.Core.Tests.Seeds;

internal static class SeedTestPaths
{
    public static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "LogicPOS.sln"))
                && Directory.Exists(Path.Combine(dir.FullName, "db_seeds")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root from test base directory.");
    }
}
