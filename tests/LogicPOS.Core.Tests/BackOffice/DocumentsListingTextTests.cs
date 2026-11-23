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
}
