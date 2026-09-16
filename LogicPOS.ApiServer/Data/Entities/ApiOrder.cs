namespace LogicPOS.ApiServer.Data.Entities;

public enum ApiOrderStatus
{
    Open = 0,
    Closed = 1,
    Deleted = 2
}

public sealed class ApiOrder
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public ApiOrderStatus Status { get; set; } = ApiOrderStatus.Open;
    public string? DeleteReason { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<ApiOrderTicket> Tickets { get; set; } = [];
}

public sealed class ApiOrderTicket
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int TicketNumber { get; set; }

    // Set only for tickets created by SplitTicket; the number of people the client should divide this ticket's total among.
    public int? SplittersNumber { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public List<ApiOrderDetail> Details { get; set; } = [];
}

public sealed class ApiOrderDetail
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid ArticleId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal Vat { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
