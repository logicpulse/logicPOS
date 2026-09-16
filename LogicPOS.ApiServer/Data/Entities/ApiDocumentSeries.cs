namespace LogicPOS.ApiServer.Data.Entities;

public sealed class ApiDocumentSeries
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int NextNumber { get; set; }
    public int NumberRangeBegin { get; set; }
    public int NumberRangeEnd { get; set; }
    public string? Acronym { get; set; }
    public Guid DocumentTypeId { get; set; }
    public Guid FiscalYearId { get; set; }

    // Never actually validated against Portugal's AT web service (no such integration exists here);
    // stored purely as free text, matching the client's optional field.
    public string? AtValidationCode { get; set; }

    public Guid? TerminalId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
