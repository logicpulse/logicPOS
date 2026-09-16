namespace LogicPOS.ApiServer.DTOs;

public sealed class ReceiptViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string RefNo { get; set; } = string.Empty;
    public string Status { get; set; } = "N";
    public decimal Amount { get; set; }
    public string? Customer { get; set; }
    public string? CustomerFiscalNumber { get; set; }
    public List<string> RelatedDocuments { get; set; } = [];
    public AgtDocumentInfoResponse Agt { get; set; } = new();
}

public sealed class ReceiptsPaginatedResponse
{
    public IEnumerable<ReceiptViewModelResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
