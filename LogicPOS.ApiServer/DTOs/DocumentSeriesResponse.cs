namespace LogicPOS.ApiServer.DTOs;

public sealed class DocumentSeriesResponse
{
    public Guid Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int NextNumber { get; set; }
    public int NumberRangeBegin { get; set; }
    public int NumberRangeEnd { get; set; }
    public string? Acronym { get; set; }
    public DocumentTypeResponse? DocumentType { get; set; }
    public FiscalYearResponse? FiscalYear { get; set; }
    public string? ATDocCodeValidationSeries { get; set; }
    public Guid? TerminalId { get; set; }
    public object? Terminal { get; set; }
}

public sealed class CreateDocumentSeriesRequest
{
    public string Designation { get; set; } = string.Empty;
    public int NextNumber { get; set; } = 1;
    public int NumberRangeBegin { get; set; } = 1;
    public int NumberRangeEnd { get; set; } = 999999;
    public string? Acronym { get; set; }
    public Guid DocumentTypeId { get; set; }
    public Guid FiscalYearId { get; set; }
    public string? AtValidationCode { get; set; }
    public string? Notes { get; set; }

    // Series-per-terminal is accepted but the created series is not restricted to any one terminal
    // (Terminals catalog isn't wired into series selection logic here); kept for shape-compatibility.
    public List<Guid>? Terminals { get; set; }
}

public sealed class UpdateDocumentSeriesRequest
{
    public string Designation { get; set; } = string.Empty;
    public string? AtValidationCode { get; set; }
}

public sealed class HasActiveDocumentSeriesResponse
{
    public bool HasActiveSeries { get; set; }
    public bool SeriesForEachTerminal { get; set; }
}
