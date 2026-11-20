using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class ListingLabelTipsTests
{
    [Theory]
    [InlineData("Fin.pedido", "Finalizar pedido")]
    [InlineData("Cond. de Pagamento", "Condições de Pagamento")]
    [InlineData("Cartão Crédito", "Cartão de Crédito")]
    [InlineData("Predefinido", "Terminal predefinido")]
    public void Maps_short_labels_to_full_term(string shortLabel, string expected)
    {
        Assert.Equal(expected, ListingLabelTips.FullTerm(shortLabel));
        Assert.Equal(expected, ListingLabelTips.Tooltip(shortLabel));
    }

    [Fact]
    public void Unknown_label_keeps_visible_text_as_tooltip()
    {
        Assert.Equal("Designação", ListingLabelTips.Tooltip("Designação"));
        Assert.Null(ListingLabelTips.FullTerm("Designação"));
    }
}