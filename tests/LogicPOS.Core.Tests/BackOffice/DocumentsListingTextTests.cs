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
    }
}
