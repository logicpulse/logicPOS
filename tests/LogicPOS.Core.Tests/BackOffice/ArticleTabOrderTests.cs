using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public class ArticleTabOrderTests
{
    [Fact]
    public void Article_tabs_match_gtk_append_order()
    {
        Assert.Equal(
            [
                "Detalhes",
                "Detalhes financeiros",
                "Outros detalhes",
                "Artigos Associados",
                "Número de série"
            ],
            BackOfficeListingService.ArticleTabOrder);
    }

    [Fact]
    public void User_tabs_keep_detail_before_contacts()
    {
        Assert.Equal(
            ["Detalhe", "Contactos", "Outros"],
            BackOfficeListingService.UserTabOrder);
    }
}
