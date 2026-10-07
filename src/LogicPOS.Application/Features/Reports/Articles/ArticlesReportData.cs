using LogicPOS.Application.Features.Reports.Common;

namespace LogicPOS.Application.Features.Reports.Articles;

public record ArticlesReportData : ReportData
{
    public List<Article> Articles { get; set; } = null!;
}

public record Article
{
    public string Code { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string Family { get; set; } = null!;
    public string FamilyCode { get; set; } = null!;
    public string Subfamily { get; set; } = null!;
    public string SubfamilyCode { get; set; } = null!;
}

