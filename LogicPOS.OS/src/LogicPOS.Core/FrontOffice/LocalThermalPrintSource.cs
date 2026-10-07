using LogicPOS.Core.Authentication;
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

        var parameters = await database.PreferenceParameters
            .AsNoTracking()
            .Select(item => new { item.Token, item.Value })
            .ToListAsync(cancellationToken);
        string? Value(string token) => parameters.FirstOrDefault(item => item.Token == token)?.Value;

        return new ThermalInvoiceJob
        {
            Printer = terminal.Printer,
            Document = document,
            TerminalName = terminal.Designation,
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

    public Task RegisterPrintAsync(Guid documentId, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
