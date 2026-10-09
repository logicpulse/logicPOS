namespace LogicPOS.Core.BackOffice;

public sealed class ReportDefinition
{
    public required string Key { get; init; }

    public required string Name { get; init; }

    public required string Group { get; init; }

    public bool Dated { get; init; } = true;

    public ReportFilterProfile FilterProfile { get; init; } = ReportFilterProfile.DateRangeOnly;

    public string? Endpoint { get; init; }

    public bool ShowDates =>
        Dated && FilterProfile is not (
            ReportFilterProfile.None
            or ReportFilterProfile.StockArticle
            or ReportFilterProfile.StockSupplier
            or ReportFilterProfile.StockArticleGain);

    public bool ShowDocumentType =>
        FilterProfile is ReportFilterProfile.DateRangeDocTerminal
            or ReportFilterProfile.DateRangeCommissions;

    public bool ShowTerminal =>
        FilterProfile is ReportFilterProfile.DateRangeDocTerminal
            or ReportFilterProfile.DateRangeAudit
            or ReportFilterProfile.DateRangeCommissions;

    public bool ShowVat => FilterProfile is ReportFilterProfile.DateRangeVat;

    public bool ShowCustomer =>
        FilterProfile is ReportFilterProfile.DateRangeCustomerOptional
            or ReportFilterProfile.DateRangeCustomerRequired
            or ReportFilterProfile.StockSupplier
            or ReportFilterProfile.StockMovements
            or ReportFilterProfile.StockArticleGain;

    public bool NeedsCustomer => FilterProfile is ReportFilterProfile.DateRangeCustomerRequired;

    public bool CustomerIsSupplier => FilterProfile is ReportFilterProfile.StockSupplier;

    public bool ShowArticle =>
        FilterProfile is ReportFilterProfile.StockWarehouse
            or ReportFilterProfile.StockArticle
            or ReportFilterProfile.SubfamilyDetailed
            or ReportFilterProfile.StockMovements
            or ReportFilterProfile.StockArticleGain;

    public bool NeedsArticle => FilterProfile is ReportFilterProfile.StockArticle;

    public bool ShowWarehouse => FilterProfile is ReportFilterProfile.StockWarehouse;

    public bool ShowFamily => FilterProfile is ReportFilterProfile.SubfamilyDetailed;

    public bool ShowSubfamily => FilterProfile is ReportFilterProfile.SubfamilyDetailed;

    public bool ShowSerial => FilterProfile is ReportFilterProfile.StockWarehouse;

    public bool ShowDocumentNumber => FilterProfile is ReportFilterProfile.StockSupplier;
}

public interface IReportService
{
    IReadOnlyList<ReportDefinition> Catalog { get; }

    Task<ListingSaveResult> GenerateAsync(
        string key,
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default);
}
