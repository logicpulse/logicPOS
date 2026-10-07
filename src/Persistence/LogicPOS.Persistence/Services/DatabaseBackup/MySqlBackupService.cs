using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Results;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

public class MySqlBackupService
{
    private readonly LogicPOSDbContext _database;
    private readonly ILogger<MySqlBackupService> _logger;

    public MySqlBackupService(LogicPOSDbContext database, ILogger<MySqlBackupService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public Result Restore()
    {
        try
        {
            string mysqlConnectionString = _database.Database.GetConnectionString()!;
            // using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
            // {
            //     using (MySqlCommand cmd = new MySqlCommand())
            //     {
            //         using (MySqlBackup mb = new MySqlBackup(cmd))
            //         {
            //             cmd.Connection = conn;
            //             conn.Open();
            //             mb.ImportFromFile(BackupInformations.BackupFilePath);
            //             conn.Close();
            //         }
            //     }
            // }
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Unexpected(ex.Message));
        }

        return Result.Success();
    }

    public Result Backup()
    {
        try
        {
            string connectionString = _database.Database.GetConnectionString()!;
            string backupFilePath = BackupSettings.BackupFilePath;
            BackupSettings.DeleteBackupFile();

            // using (MySqlConnection conn = new MySqlConnection(constring))
            // {
            //     using (MySqlCommand cmd = new MySqlCommand())
            //     {
            //         using (MySqlBackup mb = new MySqlBackup(cmd))
            //         {
            //             cmd.Connection = conn;
            //             conn.Open();
            //             mb.ExportToFile(backupFilePath);
            //             conn.Close();
            //         }
            //     }
            // }
            _logger.LogInformation("MySql backup completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while trying to backup database (mysql).");
            return Result.Failure(Error.Unexpected(ex.Message));
        }
        return Result.Success();
    }
}