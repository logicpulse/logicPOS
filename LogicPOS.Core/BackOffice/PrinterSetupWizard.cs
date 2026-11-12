using LogicPOS.Core.Authentication;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public interface IPrinterSetupWizard
{
    Task<PrinterSetupDraft> LoadAsync(CancellationToken cancellationToken = default);

    Task<PrinterSetupResult> SaveAsync(PrinterSetupRequest request, CancellationToken cancellationToken = default);

    Task<bool> HasTerminalPrinterAsync(CancellationToken cancellationToken = default);
}

public sealed class PrinterSetupType
{
    public Guid Id { get; init; }

    public string Designation { get; init; } = string.Empty;

    public bool Thermal { get; init; }
}

public sealed class PrinterSetupDraft
{
    public IReadOnlyList<PrinterSetupType> Types { get; init; } = [];

    public Guid? TypeId { get; init; }

    public string? PrinterName { get; init; }

    public string PaperLabel { get; init; } = ThermalPaper.DefaultLabel;

    public string? TerminalLabel { get; init; }

    public string? BlockedReason { get; init; }
}

public sealed class PrinterSetupRequest
{
    public Guid TypeId { get; init; }

    public string PrinterName { get; init; } = string.Empty;

    public string? PaperLabel { get; init; }
}

public sealed class PrinterSetupResult
{
    public bool Ok { get; init; }

    public string Message { get; init; } = string.Empty;

    public bool Thermal { get; init; }

    public static PrinterSetupResult Done(string message, bool thermal) => new() { Ok = true, Message = message, Thermal = thermal };

    public static PrinterSetupResult Fail(string message) => new() { Ok = false, Message = message };
}

/// <summary>
/// Creates (or reuses) a Windows printer and associates it to the current terminal:
/// thermal types go to Terminal.ThermalPrinter, A4 types to Terminal.Printer.
/// </summary>
public sealed class LocalPrinterSetupWizard : IPrinterSetupWizard
{
    private static readonly string[] WindowsTypeTokens = ["THERMAL_PRINTER_WINDOWS", "GENERIC_PRINTER_WINDOWS"];

    private readonly IServiceScopeFactory _scopes;
    private readonly IBackOfficeListingService _listing;
    private readonly ILoginService _login;
    private readonly IAuditingInformationService? _auditing;

    public LocalPrinterSetupWizard(
        IServiceScopeFactory scopes,
        IBackOfficeListingService listing,
        ILoginService login,
        IAuditingInformationService? auditing = null)
    {
        _scopes = scopes;
        _listing = listing;
        _login = login;
        _auditing = auditing;
    }

    public async Task<PrinterSetupDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var types = await database.PrinterTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false && WindowsTypeTokens.Contains(item.Token))
            .OrderBy(item => item.Order)
            .Select(item => new PrinterSetupType { Id = item.Id, Designation = item.Designation, Thermal = item.ThermalPrinter })
            .ToListAsync(cancellationToken);

        var terminalId = await ResolveTerminalIdAsync(cancellationToken);
        var terminal = terminalId is null
            ? null
            : await database.Terminals.AsNoTracking()
                .Where(item => item.Id == terminalId)
                .Select(item => new
                {
                    item.Designation,
                    Thermal = item.ThermalPrinter == null || item.ThermalPrinter.IsDeleted ? null : item.ThermalPrinter,
                    Generic = item.Printer == null || item.Printer.IsDeleted ? null : item.Printer
                })
                .FirstOrDefaultAsync(cancellationToken);
        var current = terminal?.Thermal ?? terminal?.Generic;

        return new PrinterSetupDraft
        {
            Types = types,
            TypeId = current?.TypeId ?? types.FirstOrDefault()?.Id,
            PrinterName = current?.Designation,
            PaperLabel = ThermalPaper.LabelOf(current?.ThermalMaxCharsPerLineNormal),
            TerminalLabel = terminal?.Designation,
            BlockedReason = types.Count == 0
                ? "Faltam tipos de impressora Windows na base."
                : terminal is null ? "Este posto não tem terminal registado." : null
        };
    }

    public async Task<PrinterSetupResult> SaveAsync(PrinterSetupRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.PrinterName.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return PrinterSetupResult.Fail("Escolha a impressora do Windows.");
        }

        var terminalId = await ResolveTerminalIdAsync(cancellationToken);
        if (terminalId is null)
        {
            return PrinterSetupResult.Fail("Este posto não tem terminal registado.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var type = await database.PrinterTypes.AsNoTracking()
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Id == request.TypeId, cancellationToken);
        if (type is null)
        {
            return PrinterSetupResult.Fail("Escolha o tipo de impressora.");
        }

        var paper = type.ThermalPrinter ? request.PaperLabel ?? ThermalPaper.DefaultLabel : null;
        var printer = await database.Printers
            .FirstOrDefaultAsync(item => item.IsDeleted == false && item.TypeId == type.Id && item.Designation == name, cancellationToken);
        Guid printerId;
        if (printer is null)
        {
            var fields = new Dictionary<string, string>
            {
                ["Designation"] = name,
                ["TypeId"] = type.Id.ToString()
            };
            if (paper is not null)
            {
                fields[ThermalPaper.FieldKey] = paper;
            }

            var saved = await _listing.SaveAsync("Impressoras", null, fields, cancellationToken);
            if (saved.Succeeded == false)
            {
                return PrinterSetupResult.Fail(saved.Error ?? "Não foi possível criar a impressora.");
            }

            printerId = saved.Id;
        }
        else
        {
            if (paper is not null)
            {
                var columns = ThermalPaper.Columns(paper);
                printer.ThermalMaxCharsPerLineNormal = columns.Normal;
                printer.ThermalMaxCharsPerLineNormalBold = columns.Bold;
                printer.ThermalMaxCharsPerLineSmall = columns.Small;
                printer.UpdatedAt = DateTime.Now;
            }

            printerId = printer.Id;
        }

        var terminal = await database.Terminals.FirstAsync(item => item.Id == terminalId, cancellationToken);
        if (type.ThermalPrinter)
        {
            terminal.ThermalPrinterId = printerId;
        }
        else
        {
            terminal.PrinterId = printerId;
        }

        terminal.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return PrinterSetupResult.Done($"Impressora «{name}» associada ao terminal «{terminal.Designation}».", type.ThermalPrinter);
    }

    public async Task<bool> HasTerminalPrinterAsync(CancellationToken cancellationToken = default)
    {
        var terminalId = await ResolveTerminalIdAsync(cancellationToken);
        if (terminalId is null)
        {
            return false;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.Terminals.AsNoTracking()
            .AnyAsync(item => item.Id == terminalId
                && ((item.ThermalPrinter != null && item.ThermalPrinter.IsDeleted == false)
                    || (item.Printer != null && item.Printer.IsDeleted == false)), cancellationToken);
    }

    private async Task<Guid?> ResolveTerminalIdAsync(CancellationToken cancellationToken)
    {
        var auditingId = _auditing?.GetTerminalId();
        if (auditingId is Guid id && id != Guid.Empty)
        {
            return id;
        }

        return await _login.GetFirstTerminalIdAsync(cancellationToken);
    }
}
