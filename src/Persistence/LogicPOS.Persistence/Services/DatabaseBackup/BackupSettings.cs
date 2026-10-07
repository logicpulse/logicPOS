namespace LogicPOS.Persistence.Services.DatabaseBackup;

public static class BackupSettings
{
    private static string BackupsFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "Backups");
    public const string BackupFileName = "logicposapi.bak";
    public static string BackupFilePath => Path.Combine(BackupsFolderPath, BackupFileName);
    private static string SqliteRestoreFileName => "sqlite-restore.logicpos";
    public static string SqliteRestoreFilePath => Path.Combine(BackupsFolderPath, SqliteRestoreFileName);
    
    public static void EnsureBackupsFolderExists()
    {
        if (!Directory.Exists(BackupsFolderPath))
        {
            Directory.CreateDirectory(BackupsFolderPath);
        }
    }

    public static void DeleteBackupFile()
    {
        if (File.Exists(BackupFilePath))
        {
            File.Delete(BackupFilePath);
        }
    }
    
    public static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2)
            {
                dict[kv[0].Trim()] = kv[1].Trim();
            }
        }
        return dict;
    }

}