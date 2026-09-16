namespace LogicPOS.ApiServer.DTOs;

public sealed class StockMovementItemRequest
{
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public decimal Price { get; set; }

    // Composed-article ("unique article children") linking is not built yet (ArticleHierarchy stub area); ignored.
    public List<Guid>? ChildUniqueArticles { get; set; }
}

public sealed class AddStockMovementRequest
{
    // Suppliers catalog is not built yet; stored but not validated.
    public Guid SupplierId { get; set; }

    public DateTime Date { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Notes { get; set; }
    public List<StockMovementItemRequest> Items { get; set; } = [];
    public string? ExternalDocument { get; set; }
}

public sealed class UpdateStockMovementRequest
{
    public Guid? SupplierId { get; set; }
    public DateTime? Date { get; set; }
    public string? DocumentNumber { get; set; }

    // Only meaningful when the movement has a single item (the client sends no line identifier to update).
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public string? ExternalDocument { get; set; }
}

public sealed class StockMovementResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? ExternalDocument { get; set; }
}

public sealed class StockMovementViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    // Sales/customer side is not applicable to purchase-in movements; Customers catalog isn't built either way.
    public string? Customer { get; set; }
    public Guid? CustomerId { get; set; }

    public string Article { get; set; } = string.Empty;
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public bool HasExternalDocument { get; set; }
}

public sealed class StockMovementsPaginatedResponse
{
    public IEnumerable<StockMovementViewModelResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public sealed class ArticleHistoryResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string Article { get; set; } = string.Empty;
    public decimal ArticlePrice { get; set; }
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public string WarehouseLocation { get; set; } = string.Empty;
    public Guid WarehouseLocationId { get; set; }
    public int Status { get; set; }
    public bool IsComposed { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public string? OriginDocument { get; set; }

    // No Documents/Sales subsystem exists yet: always null.
    public string? SaleDocument { get; set; }
    public Guid InMovementId { get; set; }
    public Guid? OutMovementId { get; set; }

    // No Suppliers catalog exists yet: always null (SupplierId is stored on the movement, not resolvable to a name).
    public string? Supplier { get; set; }

    public decimal PurchasePrice { get; set; }
    public bool HasExternalDocument { get; set; }
}

public sealed class ArticleHistoriesPaginatedResponse
{
    public IEnumerable<ArticleHistoryResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
