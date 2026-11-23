using System.Xml.Linq;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Tests.Seeds;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class EmptyReportMessageTests
{
    private const string European = "Sem registos. Verifique os filtros.";
    private const string Brazilian = "Sem registros. Verifique os filtros.";

    [Fact]
    public void Empty_table_pdf_uses_the_api_sentence()
    {
        var root = SeedTestPaths.FindRepoRoot();
        var source = File.ReadAllText(Path.Combine(root, "LogicPOS.Core", "BackOffice", "LocalReportService.cs"));

        Assert.Contains($"Row((\"Informação\", \"{European}\"))", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Sem dados no intervalo indicado.", source, StringComparison.Ordinal);

        var pdf = ListingTableExport.ToPdf("Relatório", ["Informação"], [[European]]);
        using var document = UglyToad.PdfPig.PdfDocument.Open(new MemoryStream(pdf));
        var text = string.Concat(document.GetPages().Select(page => page.Text));
        Assert.Contains(European, text, StringComparison.Ordinal);
    }

    [Fact]
    public void Stock_no_records_alert_uses_the_api_sentence_in_portuguese()
    {
        var root = SeedTestPaths.FindRepoRoot();
        foreach (var culture in new[] { "pt-PT", "pt-AO", "pt-MZ" })
        {
            Assert.Equal(European, ReadAlert(root, culture));
        }

        Assert.Equal(Brazilian, ReadAlert(root, "pt-BR"));

        var window = File.ReadAllText(Path.Combine(root, "LogicPOS.App", "Views", "ReportsWindow.axaml.cs"));
        Assert.Contains($"\"{European}\"", window, StringComparison.Ordinal);
        Assert.DoesNotContain("fitros", window, StringComparison.Ordinal);
    }

    private static string ReadAlert(string root, string culture)
    {
        var path = Path.Combine(root, "LogicPOS.Globalization", "Localization", $"Resx.{culture}.resx");
        var document = XDocument.Load(path);
        var value = document
            .Descendants("data")
            .Single(element => (string?)element.Attribute("name") == "dialog_message_report_filter_no_records_with_criteria")
            .Element("value")
            ?.Value;

        return value ?? throw new InvalidOperationException($"Missing alert text in {culture}.");
    }
}
