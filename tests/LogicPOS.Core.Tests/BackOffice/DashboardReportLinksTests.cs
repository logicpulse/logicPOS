using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class DashboardReportLinksTests
{
    [Fact]
    public void Top_articles_opens_total_sold_by_article()
    {
        var report = DashboardReportLinks.TopArticlesReport();

        Assert.Equal("articles-sold", report.Key);
        Assert.Equal("Total vendido por artigo", report.Name);
        Assert.NotEqual("Artigos", report.Name);
    }
}
