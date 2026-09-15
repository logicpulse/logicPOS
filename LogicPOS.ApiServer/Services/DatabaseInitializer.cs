using LogicPOS.ApiServer.Data;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class DatabaseInitializer
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await BaselineEnsureCreatedDatabaseAsync(cancellationToken);
        await _dbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task BaselineEnsureCreatedDatabaseAsync(CancellationToken cancellationToken)
    {
        var latestMigrationId = _dbContext.Database.GetMigrations().LastOrDefault();
        if (string.IsNullOrWhiteSpace(latestMigrationId))
        {
            return;
        }

        var connection = _dbContext.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        try
        {
            if (!await TableExistsAsync(connection, "ApiUsers", cancellationToken))
            {
                return;
            }

            var hasHistoryTable = await TableExistsAsync(connection, "__EFMigrationsHistory", cancellationToken);
            if (!hasHistoryTable)
            {
                await using var createHistoryCommand = connection.CreateCommand();
                createHistoryCommand.CommandText = @"
CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId TEXT NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
    ProductVersion TEXT NOT NULL
);";
                await createHistoryCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            if (await MigrationExistsAsync(connection, latestMigrationId, cancellationToken))
            {
                return;
            }

            await using var insertHistoryCommand = connection.CreateCommand();
            insertHistoryCommand.CommandText = @"
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ($migrationId, $productVersion);";

            var migrationIdParameter = insertHistoryCommand.CreateParameter();
            migrationIdParameter.ParameterName = "$migrationId";
            migrationIdParameter.Value = latestMigrationId;
            insertHistoryCommand.Parameters.Add(migrationIdParameter);

            var productVersionParameter = insertHistoryCommand.CreateParameter();
            productVersionParameter.ParameterName = "$productVersion";
            productVersionParameter.Value = "9.0.9";
            insertHistoryCommand.Parameters.Add(productVersionParameter);

            await insertHistoryCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static async Task<bool> TableExistsAsync(System.Data.Common.DbConnection connection, string tableName, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $tableName;";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "$tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is long count && count > 0;
    }

    private static async Task<bool> MigrationExistsAsync(System.Data.Common.DbConnection connection, string migrationId, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = $migrationId;";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "$migrationId";
        parameter.Value = migrationId;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is long count && count > 0;
    }
}
