using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.SalesByDate;

public record SalesByDateReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public List<SaleItem>? Items { get; set; } 
    public DateTime Date { get; set; }
    public string DocumentNumber { get; set; } = null!;
    public DateTime MovementStartTime { get; set; }
    public string Customer { get; set; } = null!;
    public string CustomerFiscalNumber { get; set; } = null!;
    public decimal FinalTotal { get; set; }
    public decimal NetTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
}

public record SaleItem
{
    public string Code { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
}