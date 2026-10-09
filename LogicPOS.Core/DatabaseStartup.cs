using LogicPOS.Application.Features.System;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core;

public static class DatabaseStartup
{
    public static void EnsureExists(DatabaseSettings settings)
    {
        if (settings.DatabaseType != DatabaseType.Sqlite)
        {
            return;
        }

        var builder = new SqliteConnectionStringBuilder(settings.ConnectionString);
        var dataSource = builder.DataSource;
        if (string.IsNullOrWhiteSpace(dataSource)
            || string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (Path.IsPathRooted(dataSource) == false)
        {
            dataSource = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dataSource));
        }

        builder.DataSource = dataSource;
        settings.ConnectionString = builder.ConnectionString;

        var directory = Path.GetDirectoryName(dataSource);
        if (string.IsNullOrWhiteSpace(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        // Deleting only logicpos.db leaves -wal/-shm behind; SQLite then fails with Error 14.
        if (File.Exists(dataSource) == false)
        {
            TryDelete(dataSource + "-wal");
            TryDelete(dataSource + "-shm");
            TryDelete(dataSource + "-journal");
        }

        // Create an empty file early so a missing/unwritable path fails here, not on login.
        builder.Mode = SqliteOpenMode.ReadWriteCreate;
        builder.Pooling = false;
        try
        {
            using var connection = new SqliteConnection(builder.ConnectionString);
            connection.Open();
            connection.Close();
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 14)
        {
            throw new InvalidOperationException(
                $"Não foi possível criar a base de dados em '{dataSource}'. " +
                "Apague logicpos.db, logicpos.db-wal e logicpos.db-shm (os três) e confirme permissão de escrita na pasta da aplicação.",
                exception);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort: Migrate may still succeed if the main file can be created.
        }
    }

    public static void ApplySchema(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var migrations = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationsService>();
        migrations.ApplyPendingMigrations();
    }

    public static async Task EnsureMachineTerminalAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var hasTerminal = await database.Terminals.AnyAsync(terminal => terminal.IsDeleted == false);
        if (hasTerminal)
        {
            await SyncOrphanLocalHardwareIdAsync(database);
            return;
        }

        var repository = scope.ServiceProvider.GetRequiredService<ITerminalRepository>();
        var references = new TerminalReferences(
            scope.ServiceProvider.GetRequiredService<IPrinterRepository>(),
            scope.ServiceProvider.GetRequiredService<IPoleDisplayRepository>(),
            scope.ServiceProvider.GetRequiredService<IWeighingMachineRepository>(),
            scope.ServiceProvider.GetRequiredService<IPlaceRepository>(),
            scope.ServiceProvider.GetRequiredService<IInputReaderRepository>());

        var created = await Terminal.CreateAsync(
            new CreateTerminalDto(
                Designation: "Terminal#",
                HardwareId: MachineIdentity.HardwareId,
                TimerInterval: 0,
                PlaceId: null,
                PrinterId: null,
                ThermalPrinterId: null,
                PoleDisplayId: null,
                WeighingMachineId: null,
                BarcodeReaderId: null,
                CardReaderId: null,
                Notes: null),
            repository,
            references);

        if (created.IsFailure)
        {
            throw new InvalidOperationException(created.Error.Message);
        }

        var terminal = created.Value!;
        terminal.Designation += terminal.Code;
        terminal.IsDefault = true;
        await database.Terminals.AddAsync(terminal);
        await database.SaveChangesAsync();
    }

    /// <summary>
    /// Older builds stored a random Guid from <c>hardware.id</c> on the terminal.
    /// When the real machine fingerprint is wired and no terminal matches it, reclaim
    /// the default/orphan local Guid so login and licence Hardware ID agree.
    /// </summary>
    private static async Task SyncOrphanLocalHardwareIdAsync(LogicPOSDbContext database)
    {
        var hardwareId = MachineIdentity.HardwareId;
        if (string.IsNullOrWhiteSpace(hardwareId))
        {
            return;
        }

        var alreadyBound = await database.Terminals
            .AnyAsync(terminal => terminal.IsDeleted == false && terminal.HardwareId == hardwareId);
        if (alreadyBound)
        {
            return;
        }

        var terminals = await database.Terminals
            .Where(terminal => terminal.IsDeleted == false)
            .OrderByDescending(terminal => terminal.IsDefault)
            .ThenBy(terminal => terminal.Code)
            .ToListAsync();
        var orphan = terminals.FirstOrDefault(terminal => IsLocalGuidHardwareId(terminal.HardwareId))
            ?? terminals.FirstOrDefault(terminal => string.IsNullOrWhiteSpace(terminal.HardwareId));
        if (orphan is null)
        {
            return;
        }

        orphan.HardwareId = hardwareId;
        orphan.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync();
    }

    private static bool IsLocalGuidHardwareId(string? value)
        => value is { Length: 32 } && value.All(Uri.IsHexDigit);

    public static string ResolveSeedPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "db_seeds");
            var usersSeed = Path.Combine(candidate, "modules", "default", "users.json");
            if (File.Exists(usersSeed))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Não encontrei a pasta db_seeds com os utilizadores por defeito.");
    }

    public static void EnsureSeed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var settings = scope.ServiceProvider.GetRequiredService<DatabaseSettings>();
        if (database.Users.Any(user => user.IsDeleted == false))
        {
            return;
        }

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        if (seeder.ApplyRequiredSeed() == false)
        {
            throw new InvalidOperationException("A semente obrigatória falhou.");
        }

        // Fresh database: always load module seed data (appsettings UseSeed only gates
        // optional extras on an already-populated DB in DatabaseInitializer).
        if (settings.UseSeed)
        {
            seeder.ApplyAdditionalSeed();
        }
        else
        {
            seeder.ApplyRequiredUsersSeed();
        }

        seeder.EnsureCountrySpecificConfiguration();
    }

    /// <summary>
    /// Creates schema + seed when the SQLite file is missing (same outcome as first install).
    /// Prefer this over calling EnsureExists / ApplySchema / EnsureSeed separately.
    /// </summary>
    public static void EnsureDatabase(IServiceProvider services, DatabaseSettings settings)
    {
        EnsureExists(settings);
        using var scope = services.CreateScope();
        var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer>();
        if (initializer is not null)
        {
            if (initializer.Initialize() == false)
            {
                throw new InvalidOperationException(
                    "Não foi possível criar ou inicializar a base de dados. Verifique permissões na pasta da aplicação e o SeedPath no appsettings.");
            }

            return;
        }

        ApplySchema(services);
        EnsureSeed(services);
    }
}
