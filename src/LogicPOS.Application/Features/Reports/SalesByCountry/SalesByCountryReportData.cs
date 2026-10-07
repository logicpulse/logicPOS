using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByCountry;


public record SalesByCountryReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public Guid CountryId { get; set; }
    public DateTime Date { get; set; }
    public string? Country { get; set; } = null!;
    public string? CountryCode2 { get; set; } = null!;
    public decimal Discount { get; set; }
    public string DocumentNumber { get; set; } = null!;
    public string CustomerFiscalNumber { get; set; } = null!;
    public string Customer { get; set; } = null!;
    public decimal FinalTotal { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public List<SaleItem>? Items { get; set; } 
}

public record SaleItem
{
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string Code { get; set; } = null!;
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}