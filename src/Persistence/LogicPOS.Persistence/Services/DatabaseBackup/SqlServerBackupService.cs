using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

/// <summary>
/// Backup files are given only a name, without a folder, so SQL Server writes them to its own default backup directory.
/// </summary>
public class SqlServerBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<SqlServerBackupService> _logger;

    public SqlServerBackupService(LogicPOSDbContext database, ILogger<SqlServerBackupService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<Result<string>> BackupAsync(string backupFileName, CancellationToken ct)
    {
        try
        {
            var sql = $"BACKUP DATABASE {GetQuotedDatabaseName()} TO DISK = {QuoteLiteral(backupFileName)} WITH INIT;";
            await _database.Database.ExecuteSqlRawAsync(sql, ct);
            _logger.LogInformation("Sql server backup completed: {BackupFileName}", backupFileName);
            return backupFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to backup database (sql server).");
            return Result.Unexpected<string>(ex.Message);
        }
    }
    
    public async Task<Result<string>> RestoreAsync(string backupFileName, CancellationToken ct)
    {
        try
        {
            var databaseName = GetQuotedDatabaseName();
            var backupDiskFileName = QuoteLiteral(backupFileName);

            var sql = $@"
                  USE MASTER; 
                  ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                  BEGIN TRY
                      RESTORE DATABASE {databaseName} FROM DISK = {backupDiskFileName} WITH REPLACE;
                  END TRY
                  BEGIN CATCH
                      ALTER DATABASE {databaseName} SET MULTI_USER;
                      THROW;
                  END CATCH;
                  ALTER DATABASE {databaseName} SET MULTI_USER;
                  USE {databaseName};
                  ";

            await _database.Database.ExecuteSqlRawAsync(sql, ct);
            _logger.LogInformation("Sql server restore completed from {BackupFileName}.", backupFileName);
            return backupFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to restore database (sql server).");
            return Result.Unexpected<string>(ex.Message);
        }
    }

    private static string QuoteLiteral(string value) => $"N'{value.Replace("'", "''")}'";

    private string GetQuotedDatabaseName() => $"[{_database.Database.GetDbConnection().Database.Replace("]", "]]")}]";
}
