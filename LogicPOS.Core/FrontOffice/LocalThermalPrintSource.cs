using LogicPOS.Core.Authentication;
using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class LocalThermalPrintSource : IThermalPrintSource
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILoginService _login;

    public LocalThermalPrintSource(IServiceScopeFactory scopes, ILoginService login)
    {
        _scopes = scopes;
        _login = login;
    }

    public async Task<ThermalInvoiceJob?> GetInvoiceJobAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var terminalId = await _login.GetFirstTerminalIdAsync(cancellationToken);
        if (terminalId is null)
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var terminal = await database.Terminals
            .AsNoTracking()
            .Where(item => item.Id == terminalId)
            .Select(item => new
            {
                item.Designation,
                Printer = item.ThermalPrinter == null || item.ThermalPrinter.IsDeleted
                    ? null
                    : new ThermalPrinterSettings
                    {
                        Designation = item.ThermalPrinter.Designation,
                        NetworkName = item.ThermalPrinter.NetworkName,
                        ColumnsNormal = item.ThermalPrinter.ThermalMaxCharsPerLineNormal ?? ThermalPrinterSettings.DefaultColumnsNormal,
                        ColumnsBold = item.ThermalPrinter.ThermalMaxCharsPerLineNormalBold ?? ThermalPrinterSettings.DefaultColumnsBold,
                        ColumnsSmall = item.ThermalPrinter.ThermalMaxCharsPerLineSmall ?? ThermalPrinterSettings.DefaultColumnsSmall
                    }
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (terminal?.Printer is null)
        {
            return null;
        }

        var document = await database.Documents
            .AsNoTracking()
            .Where(item => item.Id == documentId)
            .Select(doc => new ThermalDocument
            {
                Type = doc.Type,
                Number = doc.Number,
                Date = doc.CreatedAt,
                Currency = doc.Currency!.Acronym,
                CustomerName = doc.Customer.Name,
                CustomerAddress = doc.Customer.Address,
                CustomerZipCode = doc.Customer.ZipCode,
                CustomerCity = doc.Customer.City,
                CustomerCountry = doc.Customer.Country,
                CustomerFiscalNumber = doc.Customer.FiscalNumber,
                PaymentCondition = doc.PaymentConditionId != null ? doc.PaymentCondition!.Designation : null,
                PaymentMethods = doc.PaymentMethods!.Select(payment => payment.PaymentMethod!.Designation).ToList(),
                Lines = doc.Details!.OrderBy(detail => detail.CreatedAt).Select(detail => new ThermalDocumentLine
                {
                    Designation = detail.Designation,
                    Tax = detail.Tax.Percentage,
                    TaxDesignation = detail.Tax.Designation,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    UnitPrice = detail.Price,
                    Discount = detail.Discount,
                    TotalNet = detail.TotalNet,
                    TotalTax = detail.TotalTax,
                    TotalFinal = detail.TotalFinal,
                    VatExemptionReason = detail.VatExemptionReason
                }).ToList(),
                TotalNet = doc.TotalNet,
                TotalTax = doc.TotalTax,
                TotalFinal = doc.TotalFinal,
                AtQrCode = doc.ATQRCode
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (document is null)
        {
            return null;
        }

        var mark = FiscalMarks.Read(scope.ServiceProvider.GetService<IFiscalModule>(), new FiscalDocument
        {
            Type = document.Type,
            Number = document.Number,
            QrPayload = document.AtQrCode,
            TotalNet = document.TotalNet,
            TotalTax = document.TotalTax,
            TotalFinal = document.TotalFinal,
            Date = document.Date,
            CustomerFiscalNumber = document.CustomerFiscalNumber,
            CountryCode = document.CustomerCountry
        });
        if (string.IsNullOrWhiteSpace(mark.CodeLine) == false)
        {
            document.FiscalCodeLine = mark.CodeLine;
        }

        if (string.IsNullOrWhiteSpace(mark.QrPayload) == false)
        {
            document.AtQrCode = mark.QrPayload;
        }

        var parameters = await database.PreferenceParameters
            .AsNoTracking()
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);
        string? Value(string token) => parameters.FirstOrDefault(item => item.Token == token)?.Value;

        var openDrawer = await database.DocumentTypes
            .AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.Acronym == document.Type && item.PrintOpenDrawer, cancellationToken);

        return new ThermalInvoiceJob
        {
            Printer = terminal.Printer,
            Document = document,
            TerminalName = terminal.Designation,
            OpenDrawer = openDrawer,
            PrintQrCode = bool.TryParse(Value("PRINT_QRCODE"), out var printQr) == false || printQr,
            Company = new ThermalCompany
            {
                Name = Value("COMPANY_NAME"),
                BusinessName = Value("COMPANY_BUSINESS_NAME"),
                Address = Value("COMPANY_ADDRESS"),
                PostalCode = Value("COMPANY_POSTALCODE"),
                City = Value("COMPANY_CITY"),
                CountryCode2 = Value("COMPANY_COUNTRY_CODE2"),
                Phone = Value("COMPANY_TELEPHONE"),
                MobilePhone = Value("COMPANY_MOBILEPHONE"),
                Email = Value("COMPANY_EMAIL"),
                Website = Value("COMPANY_WEBSITE"),
                FiscalNumber = Value("COMPANY_FISCALNUMBER"),
                TicketFinalLine1 = Value("TICKET_FOOTER_LINE1"),
                TicketFinalLine2 = Value("TICKET_FOOTER_LINE2")
            }
        };
    }

    public async Task<ThermalPrinterSettings?> GetTerminalPrinterAsync(CancellationToken cancellationToken = default)
    {
        var terminalId = await _login.GetFirstTerminalIdAsync(cancellationToken);
        if (terminalId is null)
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.Terminals
            .AsNoTracking()
            .Where(item => item.Id == terminalId)
            .Select(item => item.ThermalPrinter == null || item.ThermalPrinter.IsDeleted
                ? null
                : new ThermalPrinterSettings
                {
                    Designation = item.ThermalPrinter.Designation,
                    NetworkName = item.ThermalPrinter.NetworkName,
                    ColumnsNormal = item.ThermalPrinter.ThermalMaxCharsPerLineNormal ?? ThermalPrinterSettings.DefaultColumnsNormal,
                    ColumnsBold = item.ThermalPrinter.ThermalMaxCharsPerLineNormalBold ?? ThermalPrinterSettings.DefaultColumnsBold,
                    ColumnsSmall = item.ThermalPrinter.ThermalMaxCharsPerLineSmall ?? ThermalPrinterSettings.DefaultColumnsSmall
                })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CustomerDisplaySettings?> GetCustomerDisplayAsync(CancellationToken cancellationToken = default)
    {
        var terminalId = await _login.GetFirstTerminalIdAsync(cancellationToken);
        if (terminalId is null)
        {
            return null;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var display = await database.Terminals
            .AsNoTracking()
            .Where(item => item.Id == terminalId && item.PoleDisplay != null && item.PoleDisplay.IsDeleted == false)
            .Select(item => new
            {
                item.PoleDisplay!.COMPort,
                item.PoleDisplay.CharactersPerLine,
                item.PoleDisplay.StandByLine1,
                item.PoleDisplay.StandByLine2
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (display is null || string.IsNullOrWhiteSpace(display.COMPort))
        {
            return null;
        }

        return new CustomerDisplaySettings
        {
            ComPort = display.COMPort,
            Columns = display.CharactersPerLine == 0 ? 20 : (int)display.CharactersPerLine,
            StandByLine1 = display.StandByLine1,
            StandByLine2 = display.StandByLine2
        };
    }

    public async Task<bool> IsOrderTicketEnabledAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var value = await database.PreferenceParameters
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == "TICKET_PRINT_TICKET")
            .Select(item => item.Value)
            .FirstOrDefaultAsync(cancellationToken);
        return IsOn(value);
    }

    public async Task RegisterPrintAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await AddPrintAsync(documentId, 1, null, secondPrint: false, cancellationToken);
    }

    public async Task<bool> WasPrintedAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        return await database.DocumentPrints.AsNoTracking()
            .AnyAsync(item => item.IsDeleted == false && item.DocumentId == documentId, cancellationToken);
    }

    public Task RegisterReprintAsync(Guid documentId, int copies, string reason, CancellationToken cancellationToken = default)
        => AddPrintAsync(documentId, Math.Max(1, copies), reason, secondPrint: true, cancellationToken);

    private async Task AddPrintAsync(Guid documentId, int copies, string? reason, bool secondPrint, CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        database.DocumentPrints.Add(new DocumentPrint
        {
            DocumentId = documentId,
            PrintCopies = copies,
            PrintMotive = reason,
            SecondPrint = secondPrint,
            IsThermalPrint = true,
            Designation = secondPrint ? "Segunda via" : "Original"
        });
        await database.SaveChangesAsync(cancellationToken);
    }

    private static bool IsOn(string? value)
        => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
}
