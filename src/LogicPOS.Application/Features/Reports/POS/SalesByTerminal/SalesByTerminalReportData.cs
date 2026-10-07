using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.POS.SalesByTerminal;

public record SalesByTerminalReportData : SalesReportData
{
    public List<Sale> Sales { get; set; } = null!;
}

public record Sale
{
    public Guid TerminalId { get; set; }
    public string Terminal { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Customer { get; set; } = null!;
    public string CustomerFiscalNumber { get; set; } = null!;
    public decimal NetTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public List<SaleItem>? Items { get; set; } 
    public decimal ItemsTotalQuantity => Items?.Sum(x => x.Quantity) ?? 0;
}

public record SaleItem
{
    public string Code { get; set; } = null!;
    public string Article { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal FinalTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal NetTotal { get; set; }
}