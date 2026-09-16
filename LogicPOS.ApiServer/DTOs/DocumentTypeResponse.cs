namespace LogicPOS.ApiServer.DTOs;

public sealed class DocumentTypeResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public uint Order { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public int PrintCopies { get; set; }
    public bool PrintRequestMotive { get; set; }
    public bool PrintRequestConfirmation { get; set; }
    public bool PrintOpenDrawer { get; set; }
    public int SaftDocumentType { get; set; }
}

public sealed class UpdateDocumentTypeRequest
{
    public int PrintCopies { get; set; }
    public bool PrintRequestConfirmation { get; set; }
    public bool PrintOpenDrawer { get; set; }
}
