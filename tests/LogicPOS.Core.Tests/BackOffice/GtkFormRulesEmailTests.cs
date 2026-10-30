using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public class GtkFormRulesEmailTests
{
    private static Dictionary<string, string> CustomerFields(string email) => new()
    {
        ["Name"] = "Cliente Teste",
        ["FiscalNumber"] = "B12345678",
        ["CustomerTypeId"] = Guid.NewGuid().ToString(),
        ["PriceTypeId"] = Guid.NewGuid().ToString(),
        ["CountryId"] = Guid.NewGuid().ToString(),
        ["CountryIdMeta"] = "ES",
        ["Discount"] = "0",
        ["Email"] = email
    };

    [Theory]
    [InlineData("")]
    [InlineData("cliente@empresa.pt")]
    [InlineData("a@b.com,c@d.com")]
    [InlineData("a@b.com, c@d.com")]
    [InlineData("a@b.com;c@d.com")]
    [InlineData("a@b.com; c@d.com")]
    [InlineData("first@test.pt, second@test.pt; third@test.com")]
    public void Customer_accepts_empty_or_multiple_emails(string email)
    {
        Assert.Null(GtkFormRules.Validate("Clientes", CustomerFields(email)));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@b.com,bad")]
    [InlineData("a@b.com; bad")]
    public void Customer_rejects_invalid_email_entries(string email)
    {
        var error = GtkFormRules.Validate("Clientes", CustomerFields(email));
        Assert.Equal("Indique um valor válido para o email.", error);
    }
}
