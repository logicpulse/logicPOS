namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Dashboard chart shortcuts into the report catalog. The PDF title of the target stays on the report model.
/// </summary>
public static class DashboardReportLinks
{
    public const string TopArticles = "articles-sold";

    public static ReportDefinition TopArticlesReport()
        => ReportCatalog.All.Single(item => item.Key == TopArticles);
}
