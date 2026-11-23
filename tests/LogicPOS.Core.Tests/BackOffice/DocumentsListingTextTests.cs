using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class DocumentsListingTextTests
{
    [Fact]
    public void Selected_count_follows_culture()
    {
        Assert.Equal("2 documento(s) selecionado(s)", DocumentsListingText.SelectedCount(2, "pt-PT"));
        Assert.Equal("2 document(s) selected", DocumentsListingText.SelectedCount(2, "en-US"));
    }

    [Fact]
    public void Empty_period_follows_culture()
    {
        Assert.Equal("Não existem documentos neste período.", DocumentsListingText.EmptyPeriod("pt-AO"));
        Assert.Equal("There are no documents in this period.", DocumentsListingText.EmptyPeriod("en-US"));
    }

    [Fact]
    public void Type_all_open_and_series_banner_follow_culture()
    {
        Assert.Equal("Tipo", DocumentsListingText.Type("pt-PT"));
        Assert.Equal("Type", DocumentsListingText.Type("en-US"));
        Assert.Equal("Todos", DocumentsListingText.All("pt-PT"));
        Assert.Equal("All", DocumentsListingText.All("en-US"));
        Assert.Equal("Abrir", DocumentsListingText.Open("pt-PT"));
        Assert.Equal("Open", DocumentsListingText.Open("en-US"));

        var pt = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: false, "pt-PT");
        Assert.Contains("série(s) criadas localmente", pt, StringComparison.Ordinal);
        var en = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: true, "en-US");
        Assert.Contains("series created locally", en, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("certification module", en, StringComparison.OrdinalIgnoreCase);
        var fr = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: false, "fr-FR");
        Assert.Contains("série(s) créée(s) localement", fr, StringComparison.Ordinal);
        var es = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: true, "es-ES");
        Assert.Contains("serie(s) creada(s) localmente", es, StringComparison.Ordinal);
        Assert.Contains("certificación fiscal", es, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Startup_readiness_messages_follow_culture()
    {
        Assert.Contains("ano fiscal", DocumentsListingText.MissingFiscalYear("pt-PT"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fiscal year", DocumentsListingText.MissingFiscalYear("en-US"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("2026", DocumentsListingText.MissingCurrentFiscalYear(2026, "en-US"), StringComparison.Ordinal);
        Assert.Contains("séries", DocumentsListingText.MissingDocumentSeries("pt-PT"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("series", DocumentsListingText.MissingDocumentSeries("en-US"), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Amount_paid_and_due_use_listing_local_english()
    {
        Assert.Equal("Valor Pago", DocumentsListingText.AmountPaid("pt-PT"));
        Assert.Equal("Amount Paid", DocumentsListingText.AmountPaid("en-US"));
        Assert.Equal("Valor em Dívida", DocumentsListingText.AmountDue("pt-PT"));
        Assert.Equal("Amount Due", DocumentsListingText.AmountDue("en-US"));
    }
}
