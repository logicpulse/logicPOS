namespace LogicPOS.ApiServer.DTOs;

public sealed class FiscalYearResponse
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
    public int Year { get; set; }
    public string? Acronym { get; set; }
    public bool SeriesForEachTerminal { get; set; }
}

public sealed class AddFiscalYearRequest
{
    public string Designation { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Acronym { get; set; }
    public bool SeriesForEachTerminal { get; set; }
    public string? Notes { get; set; }
}

public sealed class CloseFiscalYearRequest
{
    // Real AT/AGT submission at year-close is not implemented; accepted for shape-compatibility but has no effect.
    public bool ForceAtCommunication { get; set; } = true;
}
