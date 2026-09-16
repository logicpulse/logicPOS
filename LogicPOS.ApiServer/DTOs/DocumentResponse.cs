namespace LogicPOS.ApiServer.DTOs;

public sealed class DocumentDetailTaxResponse
{
    public Guid TaxId { get; set; }
    public string? Designation { get; set; }
    public string? Type { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal Percentage { get; set; }
    public string? CountryRegion { get; set; }
}

public sealed class DocumentDetailResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public uint Order { get; set; }
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public DocumentDetailTaxResponse? Tax { get; set; }
    public string? VatExemptionReason { get; set; }
    public string? VatExemptionCode { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalFinal { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
}

public sealed class DocumentCustomerResponse
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Locality { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public Guid? CountryId { get; set; }
    public string? FiscalNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public sealed class DocumentPaymentMethodResponse
{
    public Guid Id { get; set; }
    public Guid PaymentMethodId { get; set; }
    public PaymentMethodResponse? PaymentMethod { get; set; }
    public decimal Amount { get; set; }
}

public sealed class TaxResumeResponse
{
    public string? Code { get; set; }
    public string? Designation { get; set; }
    public decimal Base { get; set; }
    public decimal Rate { get; set; }
    public decimal Total { get; set; }
}

// Full document, matching LogicPOS.Api.Entities.Document (used by GetDocumentById / edit screens).
public sealed class DocumentFullResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public object? ShipToAddress { get; set; }
    public object? ShipFromAddress { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalFinal { get; set; }
    public DocumentCustomerResponse Customer { get; set; } = new();
    public List<DocumentPaymentMethodResponse> PaymentMethods { get; set; } = [];
    public List<DocumentDetailResponse> Details { get; set; } = [];
    public List<TaxResumeResponse> TaxResumes { get; set; } = [];
}

public sealed class AgtDocumentInfoResponse
{
    // No AGT (Angola) or AT (Portugal) tax-authority integration exists here: these are always empty,
    // meaning "not submitted" — never fabricate a validation status.
    public string? RequestId { get; set; }
    public string? ValidationStatus { get; set; }
}

// List/summary shape, matching LogicPOS.Api.Features.Finance.Documents.Documents.Common.DocumentViewModel
// (used by GetDocuments / GetUnsettledDocumentsForSettlement).
public sealed class DocumentViewModelResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public object? PaymentCondition { get; set; }
    public object? Currency { get; set; }
    public object? Customer { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "N"; // "N" = normal/active, "A" = cancelled — matches DocumentViewModel.IsActive's check
    public AgtDocumentInfoResponse Agt { get; set; } = new();
    public bool IsDraft { get; set; }
    public decimal TotalFinal { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalToPay { get; set; }
    public string? AtDocCodeId { get; set; }
    public bool AtResendDocument { get; set; }
    public List<string> RelatedDocuments { get; set; } = [];
    public bool Paid { get; set; }
}

public sealed class DocumentsPaginatedResponse
{
    public IEnumerable<DocumentViewModelResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

// --- Requests ---

public sealed class IssueDocumentDetailRequest
{
    public Guid ArticleId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public Guid? VatRateId { get; set; }
    public Guid? VatExemptionId { get; set; }
    public decimal? Discount { get; set; }
    public string? Notes { get; set; }
    public string? SerialNumber { get; set; }
}

public sealed class IssueDocumentPaymentMethodRequest
{
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}

public sealed class IssueDocumentRequest
{
    public List<IssueDocumentPaymentMethodRequest>? PaymentMethods { get; set; }
    public Guid? CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid? ParentId { get; set; }
    public decimal Discount { get; set; }
    public DocumentCustomerResponse? Customer { get; set; }
    public string? Notes { get; set; }
    public List<IssueDocumentDetailRequest>? Details { get; set; }
    public bool IsDraft { get; set; }
}

public sealed class IssueDocumentResponse
{
    public Guid Id { get; set; }

    // Always null: no AT (Portugal) tax-authority submission exists here.
    public string? AtDocCodeId { get; set; }
}

public sealed class CancelDocumentRequest
{
    public string? Reason { get; set; }
}

public sealed class PayDocumentsRequest
{
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public List<Guid> Documents { get; set; } = [];
    public string? Notes { get; set; }
}
