namespace LogicPOS.ApiServer.DTOs;

public sealed class OrderDetailResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string Designation { get; set; } = string.Empty;
    public ArticleViewModelResponse? Article { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal Vat { get; set; }

    // TotalFinal = Quantity * Price * (1 - Discount/100) * (1 + Vat/100). No business rule for this was
    // defined anywhere in the client; this is a documented, adjustable assumption.
    public decimal TotalFinal { get; set; }
}

public sealed class OrderTicketResponse
{
    public int TicketId { get; set; }
    public List<OrderDetailResponse> Details { get; set; } = [];
}

public sealed class OrderResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public TableViewModelResponse? Table { get; set; }
    public List<OrderTicketResponse> Tickets { get; set; } = [];
}

public sealed class CreateOrderDetailRequest
{
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public sealed class CreateTicketRequest
{
    public List<CreateOrderDetailRequest> Details { get; set; } = [];
}

public sealed class CreateOrderRequest
{
    public Guid TableId { get; set; }
    public List<CreateTicketRequest> Tickets { get; set; } = [];
}

public sealed class AddTicketRequest
{
    public List<CreateOrderDetailRequest> Details { get; set; } = [];
}

public sealed class ChangeOrderTableRequest
{
    public Guid NewTableId { get; set; }
}

public sealed class ReduceOrderItemRequest
{
    public Guid ArticleId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class ReduceOrderItemsRequest
{
    public List<ReduceOrderItemRequest> Items { get; set; } = [];
}

public sealed class MoveTicketItemRequest
{
    public Guid TableId { get; set; }
    public ReduceOrderItemRequest Item { get; set; } = new();
}

public sealed class SplitTicketItemRequest
{
    public Guid ArticleId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class SplitTicketRequest
{
    public int SplittersNumber { get; set; }
    public List<SplitTicketItemRequest> Items { get; set; } = [];
}
