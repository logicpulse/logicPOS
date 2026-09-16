namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiStockMovement
{
    public Guid Id { get; set; }

    // Suppliers catalog is not built yet; stored but not validated or resolved to a nested object.
    public Guid SupplierId { get; set; }

    public DateTime Date { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? ExternalDocument { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<ApiStockMovementItem> Items { get; set; } = [];
}

public sealed class ApiStockMovementItem
{
    public Guid Id { get; set; }
    public Guid StockMovementId { get; set; }
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? WarehouseLocationId { get; set; }
    public decimal Price { get; set; }
}
