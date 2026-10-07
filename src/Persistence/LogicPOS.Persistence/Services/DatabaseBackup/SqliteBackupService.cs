using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public class SqliteBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<SqliteBackupService> _logger;
    public SqliteBackupService(LogicPOSDbContext database,ILogger<SqliteBackupService> logger)
    {
        _logger = logger;
        _database = database;
    }
    
    /// <summary>
    /// Uses the SQLite online backup API so the copy is consistent and includes pages still in the WAL file.
    /// The previous backup is only replaced after the new one is complete.
    /// </summary>
    public async Task<Result> BackupSqliteAsync(CancellationToken ct)
    {
        var backupFilePath = BackupSettings.BackupFilePath;
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
            }

            File.Move(temporaryFilePath, backupFilePath, overwrite: true);
            _logger.LogInformation("Sqlite backup completed: {BackupFilePath}", backupFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to backup database (sqlite).");
            DeleteIfExists(temporaryFilePath);
            return Result.Failure(Error.Unexpected(ex.Message));
        }

        return Result.Success();
    }
    
    public Result ScheduleRestore()
    {
        try
        {
            var stream = File.CreateText(BackupSettings.SqliteRestoreFilePath);
            stream.WriteLine(BackupSettings.BackupFilePath);
            stream.Close();
            _logger.LogInformation("Sqlite restore scheduled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to schedule sqlite restore.");
            return Result.Failure(Error.Unexpected(ex.Message));
        }

        return Result.Success();
    }

    /// <summary>
    /// Restores through the SQLite backup API instead of overwriting the database file,
    /// so leftover -wal/-shm files of the old database cannot be replayed over the restored data.
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

            if (File.Exists(BackupSettings.BackupFilePath) == false)
            {
                _logger.LogWarning("Sqlite restore scheduled but backup file was not found.");
                File.Delete(BackupSettings.SqliteRestoreFilePath);
                return;
            }

            var databaseConnectionString = new SqliteConnectionStringBuilder(_database.Database.GetConnectionString())
            {
                Pooling = false
            }.ToString();

            using (var source = new SqliteConnection(BuildFileConnectionString(BackupSettings.BackupFilePath, SqliteOpenMode.ReadOnly)))
            using (var destination = new SqliteConnection(databaseConnectionString))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination);
            }

            File.Delete(BackupSettings.SqliteRestoreFilePath);
            _logger.LogInformation("Sqlite restore completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to ensure sqlite restore.");
        }
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
