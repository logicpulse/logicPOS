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

        var directory = Path.GetDirectoryName(dataSource);
        if (string.IsNullOrWhiteSpace(directory) == false)
        {
            Directory.CreateDirectory(directory);
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
        if (database.Users.Any(user => user.IsDeleted == false))
        {
            return;
        }

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        if (seeder.ApplyRequiredSeed() == false)
        {
            throw new InvalidOperationException("A semente obrigatória falhou.");
        }

        seeder.ApplyAdditionalSeed();
        seeder.EnsureCountrySpecificConfiguration();
    }
}
