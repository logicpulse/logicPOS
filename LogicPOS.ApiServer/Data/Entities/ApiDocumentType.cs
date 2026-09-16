namespace LogicPOS.ApiServer.Data.Entities;

// Mirrors LogicPOS.Api.Features.Finance.Documents.Types.Common.SaftDocumentType exactly (ordinal-compatible).
public enum ApiSaftDocumentType
{
    None = 0,
    SalesInvoices = 1,
    MovementOfGoods = 2,
    WorkingDocuments = 3,
    Payments = 4
}

// The client never creates document types (no AddDocumentType command exists) — only reads and updates print
// settings on them — so this catalog is seeded once at startup by DocumentTypeSeeder, not user-creatable.
public sealed class ApiDocumentType
{
    public Guid Id { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;

    // The short acronym (FT, FS, FR, NC, GR, ...) the client's DocumentTypeAnalyzer switches on.
    public string Acronym { get; set; } = string.Empty;

    public int PrintCopies { get; set; } = 1;
    public bool PrintRequestMotive { get; set; }
    public bool PrintRequestConfirmation { get; set; }
    public bool PrintOpenDrawer { get; set; }
    public ApiSaftDocumentType SaftDocumentType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
