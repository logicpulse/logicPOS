namespace LogicPOS.Core.BackOffice;

/// <summary>GTK / web parity: which optional filters a report shows.</summary>
public enum ReportFilterProfile
{
    None,
    DateRangeOnly,
    DateRangeDocTerminal,
    DateRangeVat,
    DateRangeCustomerOptional,
    DateRangeCustomerRequired,
    DateRangeCommissions,
    DateRangeAudit,
    SubfamilyDetailed,
    StockWarehouse,
    StockArticle,
    StockSupplier,
    StockMovements,
    StockArticleGain
}

public sealed class ReportFilterRequest
{
    public DateTime Start { get; init; }

    public DateTime End { get; init; }

    public Guid? CustomerId { get; init; }

    public Guid? ArticleId { get; init; }

    public Guid? TerminalId { get; init; }

    public Guid? DocumentTypeId { get; init; }

    /// <summary>Fiscal document acronym (FS, FT, …) when known; preferred over resolving Id.</summary>
    public string? DocumentTypeAcronym { get; init; }

    public Guid? VatRateId { get; init; }

    public Guid? WarehouseId { get; init; }

    public Guid? FamilyId { get; init; }

    public Guid? SubfamilyId { get; init; }

    public string SerialNumber { get; init; } = string.Empty;

    public string DocumentNumber { get; init; } = string.Empty;
}
