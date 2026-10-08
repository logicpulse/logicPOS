namespace LogicPOS.Core.FrontOffice;

public sealed class ThermalPrinterSettings
{
    public const int DefaultColumnsNormal = 48;
    public const int DefaultColumnsBold = 44;
    public const int DefaultColumnsSmall = 64;

    public string? Designation { get; init; }

    public string? NetworkName { get; init; }

    public int ColumnsNormal { get; init; } = DefaultColumnsNormal;

    public int ColumnsBold { get; init; } = DefaultColumnsBold;

    public int ColumnsSmall { get; init; } = DefaultColumnsSmall;

    public int PaperWidthMm => ThermalPaper.WidthOf(ColumnsNormal);
}

public sealed class CustomerDisplaySettings
{
    public string ComPort { get; init; } = string.Empty;

    public int Columns { get; init; } = 20;

    public string? StandByLine1 { get; init; }

    public string? StandByLine2 { get; init; }
}

public sealed class ThermalCompany
{
    public string? Name { get; init; }

    public string? BusinessName { get; init; }

    public string? Address { get; init; }

    public string? PostalCode { get; init; }

    public string? City { get; init; }

    public string? CountryCode2 { get; init; }

    public string? Phone { get; init; }

    public string? MobilePhone { get; init; }

    public string? Email { get; init; }

    public string? Website { get; init; }

    public string? FiscalNumber { get; init; }

    public string? TicketFinalLine1 { get; init; }

    public string? TicketFinalLine2 { get; init; }
}

public sealed class ThermalDocumentLine
{
    public string Designation { get; init; } = string.Empty;

    public decimal Tax { get; init; }

    public string? TaxDesignation { get; init; }

    public decimal Quantity { get; init; }

    public string? Unit { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Discount { get; init; }

    public decimal TotalNet { get; init; }

    public decimal TotalTax { get; init; }

    public decimal TotalFinal { get; init; }

    public string? VatExemptionReason { get; init; }
}

public sealed class ThermalDocument
{
    public string Type { get; init; } = string.Empty;

    public string Number { get; init; } = string.Empty;

    public DateTime Date { get; init; }

    public string? Currency { get; init; }

    public string? CustomerName { get; init; }

    public string? CustomerAddress { get; init; }

    public string? CustomerZipCode { get; init; }

    public string? CustomerCity { get; init; }

    public string? CustomerCountry { get; init; }

    public string? CustomerFiscalNumber { get; init; }

    public string? PaymentCondition { get; init; }

    public IReadOnlyList<string> PaymentMethods { get; init; } = [];

    public IReadOnlyList<ThermalDocumentLine> Lines { get; init; } = [];

    public decimal TotalNet { get; init; }

    public decimal TotalTax { get; init; }

    public decimal TotalFinal { get; init; }

    public string? AtQrCode { get; set; }

    public string? FiscalCodeLine { get; set; }
}

public sealed class ThermalInvoiceJob
{
    public required ThermalPrinterSettings Printer { get; init; }

    public required ThermalCompany Company { get; init; }

    public required ThermalDocument Document { get; init; }

    public bool PrintQrCode { get; init; } = true;

    public bool OpenDrawer { get; init; }

    public string? TerminalName { get; init; }

    public bool IsSecondCopy { get; init; }
}

public interface IThermalPrintSource
{
    /// <summary>Returns null when the current terminal has no thermal printer.</summary>
    Task<ThermalInvoiceJob?> GetInvoiceJobAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<ThermalPrinterSettings?> GetTerminalPrinterAsync(CancellationToken cancellationToken = default);

    /// <summary>Terminal pole display. Null when none is configured.</summary>
    Task<CustomerDisplaySettings?> GetCustomerDisplayAsync(CancellationToken cancellationToken = default);

    /// <summary>System parameter TICKET_PRINT_TICKET: print the kitchen ticket when an order is finished.</summary>
    Task<bool> IsOrderTicketEnabledAsync(CancellationToken cancellationToken = default);

    Task RegisterPrintAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<bool> WasPrintedAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task RegisterReprintAsync(Guid documentId, int copies, string reason, CancellationToken cancellationToken = default);
}
