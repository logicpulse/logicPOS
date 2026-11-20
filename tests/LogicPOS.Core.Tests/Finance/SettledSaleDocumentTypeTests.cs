using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.Finance;

public sealed class SettledSaleDocumentTypeTests
{
    private static PosDocumentRules.SettledSaleContext Ctx(
        string? module,
        decimal total,
        bool artigo53 = false,
        bool ambulante = false,
        bool allProducts = true,
        string? buyerNif = null)
        => new(module, total, false, artigo53, ambulante, allProducts, buyerNif);

    [Theory]
    [InlineData("restaurant", 80, false, "FS")]
    [InlineData("restaurant", 100, false, "FS")]
    [InlineData("restaurant", 100.01, false, "FR")]
    [InlineData("cafe", 50, false, "FS")]
    [InlineData("cafe", 150, false, "FR")]
    [InlineData("parking", 80, false, "FS")]
    [InlineData("parking", 150, false, "FR")]
    [InlineData("restaurant", 200, true, "FR")]
    public void Resolve_settled_sale_type_from_mode_and_total(string? module, decimal total, bool angola, string expected)
    {
        if (angola)
        {
            Assert.Equal(expected, PosDocumentRules.ResolveSettledSaleDocumentType(
                new PosDocumentRules.SettledSaleContext(module, total, true, false, false, true, null)));
            return;
        }

        Assert.Equal(expected, PosDocumentRules.ResolveSettledSaleDocumentType(Ctx(module, total)));
    }

    [Theory]
    [InlineData("bakery", 900, true, null, "FS")]
    [InlineData("bakery", 1000, true, null, "FS")]
    [InlineData("bakery", 1000.01, true, null, "FR")]
    [InlineData("retail", 500, true, null, "FS")]
    [InlineData("butchery", 80, false, "S", "FS")]
    [InlineData("butchery", 150, false, "S", "FR")]
    [InlineData("butchery", 1000.01, false, null, "FR")]
    public void Retail_cap_goods_only(string module, decimal total, bool allProducts, string? mixedClass, string expected)
    {
        var classes = allProducts
            ? new[] { "P" }
            : new[] { "P", mixedClass ?? "S" };
        Assert.Equal(expected, PosDocumentRules.ResolveSettledSaleDocumentType(new PosDocumentRules.SettledSaleContext(
            module, total, false, false, false, PosDocumentRules.AllLinesAreProducts(classes), null)));
    }

    [Theory]
    [InlineData("bakery", 1500, "512345678")]
    [InlineData("restaurant", 1500, "512345678")]
  public void Sujeito_passivo_caps_at_one_hundred(string module, decimal total, string nif)
    {
        Assert.Equal("FS", PosDocumentRules.ResolveSettledSaleDocumentType(Ctx(module, 80, buyerNif: nif)));
        Assert.Equal("FR", PosDocumentRules.ResolveSettledSaleDocumentType(Ctx(module, 150, buyerNif: nif)));
        Assert.Equal(PosDocumentRules.ServiceSimplifiedInvoiceCap, PosDocumentRules.ResolveSimplifiedInvoiceCap(Ctx(module, total, buyerNif: nif)));
    }

    [Fact]
    public void Artigo53_removes_cap()
    {
        var context = Ctx("restaurant", 5000, artigo53: true);
        Assert.Null(PosDocumentRules.ResolveSimplifiedInvoiceCap(context));
        Assert.Equal("FS", PosDocumentRules.ResolveSettledSaleDocumentType(context));
    }

    [Fact]
    public void Ambulante_opens_thousand_outside_service_modes()
    {
        var context = Ctx("default", 900, ambulante: true);
        Assert.Equal(PosDocumentRules.RetailSimplifiedInvoiceCap, PosDocumentRules.ResolveSimplifiedInvoiceCap(context));
        Assert.Equal("FS", PosDocumentRules.ResolveSettledSaleDocumentType(Ctx("default", 900, ambulante: true)));
    }

    [Fact]
    public void Ambulante_does_not_raise_restaurant_cap()
    {
        Assert.Equal(PosDocumentRules.ServiceSimplifiedInvoiceCap, PosDocumentRules.ResolveSimplifiedInvoiceCap(Ctx("restaurant", 50, ambulante: true)));
    }

    [Theory]
    [InlineData("restaurant", true)]
    [InlineData("cafe", true)]
    [InlineData("parking", true)]
    [InlineData("Café", true)]
    [InlineData("bakery", false)]
    public void Service_hundred_cap_applies_to_service_modes(string module, bool capped)
    {
        Assert.Equal(capped, PosDocumentRules.UsesServiceHundredCap(module));
    }

    [Theory]
    [InlineData("FR", false)]
    [InlineData("FT", true)]
    [InlineData("FS", false)]
    public void Fatura_recibo_does_not_require_named_customer(string acronym, bool required)
    {
        Assert.Equal(required, PosDocumentRules.RequiresNamedCustomer(acronym));
    }

    [Fact]
    public void Company_nif_above_cap_requires_named_customer()
    {
        var context = Ctx("restaurant", 150, buyerNif: "512345678");
        Assert.NotNull(PosDocumentRules.ValidateCompanyBuyerForSettledSale(context, null, null, null, true));
        Assert.Null(PosDocumentRules.ValidateCompanyBuyerForSettledSale(
            context,
            "Empresa",
            "Rua A",
            "512345678",
            false));
    }

    [Theory]
    [InlineData("999999990", true, null)]
    [InlineData("123456789", true, "123456789")]
    [InlineData("512345678", false, "512345678")]
    public void Invoice_face_nif_rules(string nif, bool finalConsumer, string? expected)
    {
        Assert.Equal(expected, PosDocumentRules.ResolveInvoiceFaceFiscalNumber(nif, finalConsumer, nif));
    }

    [Theory]
    [InlineData("999999990", null)]
    [InlineData("123456789", "123456789")]
    public void Thermal_hides_final_consumer_generic_nif(string stored, string? face)
    {
        Assert.Equal(face, PosDocumentRules.ThermalCustomerFiscalNumber(stored));
    }

    [Fact]
    public void Can_issue_simplified_refused_outside_cap()
    {
        var context = Ctx("bakery", 1500);
        Assert.False(PosDocumentRules.CanIssueSimplifiedInvoice(context));
    }
}