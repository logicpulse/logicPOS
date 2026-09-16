namespace LogicPOS.ApiServer.Data.Entities;

public enum ApiArticleSerialNumberStatus
{
    None = 0,
    Available = 1,
    Sold = 2,
    Exchanged = 3,
    Returned = 4
}

public sealed class ApiWarehouseArticle
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string? SerialNumber { get; set; }
    public ApiArticleSerialNumberStatus Status { get; set; } = ApiArticleSerialNumberStatus.Available;
    public Guid WarehouseLocationId { get; set; }
    public decimal Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
