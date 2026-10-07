using System.Globalization;

namespace LogicPOS.Core.BackOffice;

public sealed class StockPageRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public string? Search { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public Guid? ArticleId { get; init; }

    public Guid? CustomerId { get; init; }

    public string? Status { get; init; }
}

public sealed class StockSerialChoice
{
    public Guid ArticleId { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Designation { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;
}

public sealed class StockPageResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int TotalPages { get; init; }

    public int TotalCount { get; init; }
}

public sealed class StockArticleRow
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Designation { get; init; } = string.Empty;

    public decimal TotalStock { get; init; }

    public decimal MinimumStock { get; init; }

    public string Unit { get; init; } = string.Empty;

    public DateTime? UpdatedAt { get; init; }
}

public sealed class StockMovementRow
{
    public Guid Id { get; init; }

    public DateTime Date { get; init; }

    public string Customer { get; init; } = string.Empty;

    public Guid? CustomerId { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Article { get; init; } = string.Empty;

    public Guid ArticleId { get; init; }

    public string SerialNumber { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public bool HasExternalDocument { get; init; }

    public bool HasSaleDocument { get; init; }

    public string? SaleDocumentNumber { get; init; }

    public bool CanEdit { get; init; }

    public string Movement => Quantity < 0 ? "Saída" : "Entrada";

    public DateTime? UpdatedAt { get; init; }
}

public sealed class StockHistoryRow
{
    public Guid Id { get; init; }

    public Guid ArticleId { get; init; }

    public string Article { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public bool IsComposed { get; init; }

    public bool IsSold { get; init; }

    public string Warehouse { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public Guid WarehouseLocationId { get; init; }

    public Guid WarehouseArticleId { get; init; }

    public string Supplier { get; init; } = string.Empty;

    public decimal PurchasePrice { get; init; }

    public decimal ArticlePrice { get; init; }

    public DateTime? PurchaseDate { get; init; }

    public DateTime? SaleDate { get; init; }

    public string? OriginDocument { get; init; }

    public string? SaleDocument { get; init; }

    public Guid InMovementId { get; init; }

    public Guid? OutMovementId { get; init; }

    public bool HasExternalDocument { get; init; }

    public string SoldText => IsSold ? "Sim" : "Não";

    public string ComposedText => IsComposed ? "Sim" : "Não";

    public DateTime? UpdatedAt { get; init; }
}

public sealed class StockWarehouseRow
{
    public Guid Id { get; init; }

    public Guid ArticleId { get; init; }

    public string Article { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string Warehouse { get; init; } = string.Empty;

    public Guid LocationId { get; init; }

    public string Location { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime? UpdatedAt { get; init; }
}

public sealed class StockMovementLineInput
{
    public Guid ArticleId { get; init; }

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public string? SerialNumber { get; init; }

    public Guid? WarehouseLocationId { get; init; }

    public IReadOnlyList<Guid>? ChildUniqueArticles { get; init; }
}

public sealed class StockMovementCreateRequest
{
    public Guid SupplierId { get; init; }

    public DateTime Date { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public byte[]? ExternalDocument { get; init; }

    public IReadOnlyList<StockMovementLineInput> Items { get; init; } = [];
}

public sealed class StockMovementUpdateRequest
{
    public Guid Id { get; init; }

    public Guid? SupplierId { get; init; }

    public DateTime? Date { get; init; }

    public string? DocumentNumber { get; init; }

    public decimal? Quantity { get; init; }

    public decimal? Price { get; init; }

    public byte[]? ExternalDocument { get; init; }
}

public sealed class StockLocationOption
{
    public Guid Id { get; init; }

    public Guid WarehouseId { get; init; }

    public string Warehouse { get; init; } = string.Empty;

    public string Designation { get; init; } = string.Empty;

    public bool IsDefault { get; init; }

    public string Label => string.IsNullOrWhiteSpace(Warehouse)
        ? Designation
        : $"{Warehouse} / {Designation}";

    public override string ToString() => Label;
}

public sealed class StockCompositionSlot
{
    public Guid ArticleId { get; init; }

    public string Article { get; init; } = string.Empty;

    public Guid? ChildId { get; init; }

    public string SerialNumber { get; init; } = string.Empty;
}

public sealed class StockSaleDocumentOption
{
    public Guid CustomerId { get; init; }

    public string Number { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public override string ToString() => Label;
}

public sealed class StockModuleAccess
{
    public bool HasModule { get; init; } = true;

    public bool ShowAcquireMessage { get; init; }
}

public sealed class StockDashboardPoint
{
    public string Label { get; init; } = string.Empty;

    public double Value { get; init; }
}

public sealed class StockDashboardSnapshot
{
    public int Year { get; init; }

    public int BelowMinimum { get; init; }

    public int UniqueInStock { get; init; }

    public int UniqueSoldInYear { get; init; }

    public int UniqueComposedInStock { get; init; }

    public decimal StockValue { get; init; }

    public bool Limited { get; init; }

    public IReadOnlyList<StockDashboardPoint> Warehouses { get; init; } = [];

    public IReadOnlyList<StockDashboardPoint> TopSold { get; init; } = [];
}

public static class StockDashboard
{
    public const int PageSize = 100;

    public const int MaxPages = 20;

    public static StockDashboardSnapshot Compose(
        int year,
        IReadOnlyList<StockArticleRow> articles,
        IReadOnlyList<StockHistoryRow> history,
        IReadOnlyList<StockWarehouseRow> warehouses,
        IReadOnlyList<StockMovementRow> movements,
        bool limited)
    {
        var inStock = history.Where(item => item.IsSold == false).ToList();
        return new StockDashboardSnapshot
        {
            Year = year,
            BelowMinimum = articles.Count(item => item.MinimumStock > 0 && item.TotalStock < item.MinimumStock),
            UniqueInStock = inStock.Count,
            UniqueSoldInYear = history.Count(item => item.SaleDate is DateTime sale && sale.Year == year),
            UniqueComposedInStock = inStock.Count(item => item.IsComposed),
            StockValue = inStock.Sum(item => item.PurchasePrice),
            Limited = limited,
            Warehouses = Rank(
                history.Where(item => item.IsSold == false)
                    .Select(item => (Label: item.Warehouse, Value: 1m))
                    .Concat(warehouses
                        .Where(item => string.IsNullOrWhiteSpace(item.SerialNumber) && item.Quantity > 0)
                        .Select(item => (Label: item.Warehouse, Value: item.Quantity))),
                8),
            TopSold = Rank(
                history.Where(item => item.SaleDate is DateTime sale && sale.Year == year)
                    .Select(item => (Label: item.Article, Value: 1m))
                    .Concat(movements
                        .Where(item => item.Quantity < 0 && string.IsNullOrWhiteSpace(item.SerialNumber))
                        .Select(item => (Label: item.Article, Value: -item.Quantity))),
                8)
        };
    }

    public static IReadOnlyList<StockDashboardPoint> Rank(IEnumerable<(string Label, decimal Value)> rows, int take)
        => rows
            .GroupBy(row => string.IsNullOrWhiteSpace(row.Label) ? "—" : row.Label.Trim(), StringComparer.CurrentCultureIgnoreCase)
            .Select(group =>
            {
                var total = group.Sum(row => row.Value);
                return new StockDashboardPoint
                {
                    Label = Short(group.Key) + " (" + total.ToString("0", CultureInfo.CurrentCulture) + ")",
                    Value = (double)total
                };
            })
            .Where(point => point.Value > 0)
            .OrderByDescending(point => point.Value)
            .Take(take)
            .ToList();

    private static string Short(string label)
        => label.Length <= 18 ? label : label[..17] + "…";
}

public interface IStockManagementService
{
    Task<StockPageResult<StockArticleRow>> GetArticlesAsync(StockPageRequest request, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> SaveMinimumStockAsync(Guid articleId, decimal minimumStock, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> AdjustArticleStockAsync(Guid articleId, decimal newTotal, CancellationToken cancellationToken = default);

    Task<StockPageResult<StockMovementRow>> GetMovementsAsync(StockPageRequest request, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> CreateMovementAsync(StockMovementCreateRequest request, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> UpdateMovementAsync(StockMovementUpdateRequest request, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> DeleteMovementAsync(Guid movementId, CancellationToken cancellationToken = default);

    Task<string?> SaveExternalDocumentAsync(Guid movementId, CancellationToken cancellationToken = default);

    Task<string?> SaveSaleDocumentPdfAsync(string documentNumber, CancellationToken cancellationToken = default);

    Task<StockPageResult<StockHistoryRow>> GetHistoryAsync(StockPageRequest request, CancellationToken cancellationToken = default);

    Task<string?> GenerateBarcodeLabelsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);

    Task<string?> GenerateBarcodeLabelsBySerialAsync(IReadOnlyList<string> serialNumbers, CancellationToken cancellationToken = default);

    Task<StockModuleAccess> GetModuleAccessAsync(CancellationToken cancellationToken = default);

    Task<StockDashboardSnapshot> LoadDashboardAsync(int year, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockCompositionSlot>> GetCompositionSlotsAsync(Guid warehouseArticleId, Guid articleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockSaleDocumentOption>> LookupSaleDocumentsAsync(CancellationToken cancellationToken = default);

    Task<ListingSaveResult> ChangeLocationAsync(Guid warehouseArticleId, Guid locationId, decimal quantity, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> UpdateUniqueArticleAsync(Guid warehouseArticleId, string? serialNumber, IReadOnlyList<Guid>? children, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> ExchangeUniqueArticleAsync(Guid returnedId, Guid exchangeId, CancellationToken cancellationToken = default);

    Task<ListingSaveResult> DeleteSerialAsync(Guid warehouseArticleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LookupOption>> GetAvailableSerialsAsync(Guid articleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockSerialChoice>> SearchAvailableSerialsAsync(string? search, CancellationToken cancellationToken = default);

    Task<StockPageResult<StockWarehouseRow>> GetWarehouseArticlesAsync(StockPageRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockLocationOption>> GetLocationsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LookupOption>> LookupArticlesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LookupOption>> LookupSuppliersAsync(CancellationToken cancellationToken = default);
}
