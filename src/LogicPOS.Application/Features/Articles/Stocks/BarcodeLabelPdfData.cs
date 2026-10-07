namespace LogicPOS.Application.Features.Articles.Stocks;

public record BarcodeLabelPdfData
{
    public string? CompanyWebsite { get; set; }
    public string? PrintModel { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public string Code { get; set; } = null!;
}

