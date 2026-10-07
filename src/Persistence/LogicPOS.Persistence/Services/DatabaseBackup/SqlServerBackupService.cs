using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public class SqlServerBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<SqlServerBackupService> _logger;

    public SqlServerBackupService(LogicPOSDbContext database, ILogger<SqlServerBackupService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<Result> BackupAsync(CancellationToken ct)
    {
        try
        {
            BackupSettings.DeleteBackupFile();
            var databaseName = GetDatabaseName();
            var sql = $"BACKUP DATABASE {databaseName} TO DISK = '{BackupSettings.BackupFileName}' WITH INIT;";
            await _database.Database.ExecuteSqlRawAsync(sql, ct);
            _logger.LogInformation("Sql server backup completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to backup database (sql server).");
            return Result.Failure(Error.Unexpected(ex.Message));
        }

        return Result.Success();
    }
    
    public async Task<Result> RestoreAsync(CancellationToken ct)
    {
        try
        {
            var backupFilePath = BackupSettings.BackupFilePath;
            var databaseName = GetDatabaseName();

            var sql = $@"
                  USE MASTER; 
                  ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                  RESTORE DATABASE {databaseName} FROM DISK = '{BackupSettings.BackupFileName}';
                  ALTER DATABASE {databaseName} SET MULTI_USER;
                  USE {databaseName};
                  ";

            await _database.Database.ExecuteSqlRawAsync(sql, ct);
            _logger.LogInformation("Sql server restore completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to restore database (sql server).");
            return Result.Failure(Error.Unexpected(ex.Message));
        }

        return Result.Success();
    }
    
    private string GetDatabaseName() => BackupSettings.ParseConnectionString(_database.Database.GetConnectionString()!)["Database"];
}