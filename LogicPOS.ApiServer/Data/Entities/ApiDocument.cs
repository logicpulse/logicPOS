namespace LogicPOS.ApiServer.Data.Entities;

public enum ApiDocumentStatus
{
    Draft = 0,
    Active = 1,
    Cancelled = 2
}

public sealed class ApiDocument
{
    public Guid Id { get; set; }

    // Customers catalog is not built (the client can issue documents to an ad-hoc customer without one via
    // the embedded snapshot fields below); stored but never validated against a real table.
    public Guid? CustomerId { get; set; }

    public string Type { get; set; } = string.Empty; // DocumentType.Acronym snapshot, e.g. "FT", "FS", "FR", "NC"

    // Null only for Draft documents, which are never numbered (see IssueAsync).
    public Guid? DocumentSeriesId { get; set; }
    public string Number { get; set; } = string.Empty; // "<SeriesAcronym> <Number>/<Year>", assigned atomically on issue

    public Guid? OrderId { get; set; } // set when issued from an Order via IssueDocument
    public Guid? ParentId { get; set; } // e.g. the invoice a credit note (NC) corrects

    // Customer snapshot: invoices must preserve customer data as it was at issue time, not link live to a
    // mutable record, so this is captured directly rather than resolved from a Customers catalog.
    public string? CustomerName { get; set; }
    public string? CustomerFiscalNumber { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerLocality { get; set; }
    public string? CustomerZipCode { get; set; }
    public string? CustomerCity { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    public decimal Discount { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalFinal { get; set; }
    public decimal TotalPaid { get; set; }

    public ApiDocumentStatus Status { get; set; } = ApiDocumentStatus.Draft;
    public string? CancelReason { get; set; }

    // Comma-separated document Numbers this document settles (set on a Receipt/"RC" issued via PayDocuments)
    // or corrects (set on a credit note). No join table exists for this; kept simple since it's read-only display data.
    public string? RelatedDocumentNumbers { get; set; }

    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<ApiDocumentDetail> Details { get; set; } = [];
    public List<ApiDocumentPaymentMethod> PaymentMethods { get; set; } = [];
}

public sealed class ApiDocumentDetail
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public uint Order { get; set; }
    public Guid ArticleId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    // Tax snapshot at issue time (never re-resolved from VatRates later, for the same reason customer data is snapshotted).
    public Guid? VatRateId { get; set; }
    public string? VatDesignation { get; set; }
    public string? VatCode { get; set; }
    public decimal VatPercentage { get; set; }

    public Guid? VatExemptionReasonId { get; set; }
    public string? VatExemptionReason { get; set; }

    public decimal TotalNet { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalFinal { get; set; }
    public string? SerialNumber { get; set; }
    public string? Warehouse { get; set; }
}

public sealed class ApiDocumentPaymentMethod
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}
