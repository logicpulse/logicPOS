using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public class SqliteBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly BackupFiles _backupFiles;
    private readonly ILogger<SqliteBackupService> _logger;
    public SqliteBackupService(LogicPOSDbContext database, BackupFiles backupFiles, ILogger<SqliteBackupService> logger)
    {
        _logger = logger;
        _database = database;
        _backupFiles = backupFiles;
    }
    
    /// <summary>
    /// Uses the SQLite online backup API so the copy is consistent and includes pages still in the WAL file.
    /// The file only gets its final name after it is complete and passes an integrity check,
    /// so a corrupted database never leaves a backup that looks valid.
    /// </summary>
    public async Task<Result<string>> BackupSqliteAsync(string backupFileName, CancellationToken ct)
    {
        _backupFiles.EnsureFolderExists();
        var backupFilePath = _backupFiles.GetFilePath(backupFileName);
        var temporaryFilePath = backupFilePath + ".tmp";

        try
        {
            DeleteIfExists(temporaryFilePath);

            await using (var source = new SqliteConnection(_database.Database.GetConnectionString()))
            await using (var destination = new SqliteConnection(BuildFileConnectionString(temporaryFilePath, SqliteOpenMode.ReadWriteCreate)))
            {
                await source.OpenAsync(ct);
                await destination.OpenAsync(ct);
                source.BackupDatabase(destination);
                EnsureIntegrity(destination);
                // Without this the copy stays in WAL mode and every read-only open leaves -wal/-shm files next to it.
                ExecuteNonQuery(destination, "PRAGMA journal_mode=DELETE;");
            }

            File.Move(temporaryFilePath, backupFilePath);
            _logger.LogInformation("Sqlite backup completed: {BackupFilePath}", backupFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to backup database (sqlite).");
            DeleteIfExists(temporaryFilePath);
            return Result.Unexpected<string>(ex.Message);
        }

        return backupFilePath;
    }
    
    /// <summary>
    /// The backups folder is looked up first because PATH_BACKUPS may have changed since the backup was made.
    /// </summary>
    public string? FindBackupFilePath(SystemBackup backup) =>
        new[] { _backupFiles.FolderPath, backup.FilePath }
            .Where(folderPath => string.IsNullOrWhiteSpace(folderPath) == false)
            .Select(folderPath => Path.Combine(folderPath!, backup.FileName!))
            .FirstOrDefault(File.Exists);

    /// <summary>
    /// For when the SystemBackups table has no usable row. Backups made by 1.5 versions before the 1.4 naming
    /// came back are only used when no newer backup exists.
    /// </summary>
    public string? FindLatestLocalFilePath() =>
        _backupFiles.FindHighestVersionFilePath()
        ?? (File.Exists(BackupSettings.BackupFilePath) ? BackupSettings.BackupFilePath : null);

    /// <summary>
    /// Restores into the live database through the SQLite backup API, so connections the API already has open
    /// see the restored data right away and leftover -wal/-shm files cannot be replayed over it.
    /// </summary>
    public Result<string> RestoreSqlite(string? backupFilePath)
    {
        if (backupFilePath == null)
        {
            _logger.LogWarning("Sqlite restore requested but no backup was found in {BackupsFolder}.", _backupFiles.FolderPath);
            return Result.NotFound<string>("Backup");
        }

        try
        {
            RestoreFromBackupFile(backupFilePath);
            _logger.LogInformation("Sqlite restore completed from {BackupFilePath}.", backupFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to restore database (sqlite).");
            return Result.Unexpected<string>(ex.Message);
        }

        return backupFilePath;
    }

    /// <summary>
    /// Discards a restore left pending by older versions, which only restored on the next API start.
    /// That start can come days later, so applying it would silently replace everything recorded since the request.
    /// </summary>
    public void EnsureSqliteRestore()
    {
        try
        {
            if (File.Exists(BackupSettings.SqliteRestoreFilePath) == false)
            {
                _logger.LogDebug("Sqlite restore file not found.");
                return;
            }

            File.Delete(BackupSettings.SqliteRestoreFilePath);
            _logger.LogWarning(
                "Discarded a sqlite restore left pending by an older version. The backup file was kept: {BackupFilePath}",
                BackupSettings.BackupFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to discard a pending sqlite restore.");
        }
    }

    private void RestoreFromBackupFile(string backupFilePath)
    {
        var databaseConnectionString = new SqliteConnectionStringBuilder(_database.Database.GetConnectionString())
        {
            Pooling = false
        }.ToString();

        using var source = new SqliteConnection(BuildFileConnectionString(backupFilePath, SqliteOpenMode.ReadOnly));
        using var destination = new SqliteConnection(databaseConnectionString);
        source.Open();
        EnsureIntegrity(source);
        destination.Open();
        source.BackupDatabase(destination);
    }

    private static void EnsureIntegrity(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA quick_check;";
        var result = Convert.ToString(command.ExecuteScalar());
        if (string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase) == false)
        {
            throw new InvalidOperationException($"SQLite integrity check failed: {result}");
        }
    }

    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string BuildFileConnectionString(string filePath, SqliteOpenMode mode) =>
        new SqliteConnectionStringBuilder
        {
            DataSource = filePath,
            Mode = mode,
            Pooling = false
        }.ToString();

    private static void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
