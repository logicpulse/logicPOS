namespace LogicPOS.ApiServer.DTOs;

public sealed class WarehouseArticleResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public ArticleResponse? Article { get; set; }
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; }
    public int Status { get; set; }
    public WarehouseLocationResponse? WarehouseLocation { get; set; }
    public Guid WarehouseLocationId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class WarehouseArticleViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public Guid WarehouseId { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public decimal Quantity { get; set; }
    public Guid ArticleId { get; set; }
}

public sealed class WarehouseArticlesPaginatedResponse
{
    public IEnumerable<WarehouseArticleViewModelResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public sealed class TotalStockResponse
{
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class ChangeArticleLocationRequest
{
    public Guid WarehouseArticleId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Quantity { get; set; }
}
