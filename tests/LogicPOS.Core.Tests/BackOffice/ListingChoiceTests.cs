using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class ListingChoiceTests
{
    [Fact]
    public void Listing_choice_shows_label_and_keeps_value()
    {
        var choice = new ListingChoice(nameof(SaftDocumentType.SalesInvoices), "Documentos comerciais a clientes");
        Assert.Equal(nameof(SaftDocumentType.SalesInvoices), choice.Value);
        Assert.Equal("Documentos comerciais a clientes", choice.ToString());
    }

    [Fact]
    public void Saft_and_stock_enums_expose_expected_names()
    {
        Assert.Contains(nameof(SaftDocumentType.SalesInvoices), Enum.GetNames<SaftDocumentType>());
        Assert.Contains(nameof(ProcessArticleStockMode.Out), Enum.GetNames<ProcessArticleStockMode>());
        Assert.Contains(nameof(ProcessArticleStockMode.In), Enum.GetNames<ProcessArticleStockMode>());
    }
}
