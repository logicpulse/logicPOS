using System.Text.Json;
using LogicPOS.Domain.Entities.Articles.Articles.Common;

namespace LogicPOS.Core.Tests.Seeds;

public sealed class DemoSeedTextTests
{
    [Fact]
    public void SdrConstants_Deposit_Designation_IsDeposito()
    {
        Assert.Equal("Depósito", SdrConstants.SdrArticleDesignation);
        Assert.Equal("SDRVDEP", SdrConstants.SdrArticleCode);
    }

    [Fact]
    public void PermissionItems_StockGain_HasTranslatedDesignation()
    {
        var items = ReadArray("permissionitems.json");
        var stockGain = items.Single(item => item.GetProperty("Token").GetString() == "REPORT_LIST_STOCK_GAIN");
        var designation = stockGain.GetProperty("Designation").GetString();
        Assert.False(string.IsNullOrWhiteSpace(designation));
        Assert.DoesNotContain("__designation[", designation, StringComparison.Ordinal);
        Assert.Contains("valorização", designation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PaymentMethods_VirtualMoney_UsesEuropeanPortuguese()
    {
        var items = ReadArray("paymentmethods.json");
        var virtualMoney = items.Single(item => item.GetProperty("Token").GetString() == "VIRTUAL_MONEY");
        Assert.Equal("Dinheiro Eletrónico", virtualMoney.GetProperty("Designation").GetString());
    }

    [Fact]
    public void PreferenceParameters_SmtpCredentials_AreEmptyByDefault()
    {
        foreach (var culture in new[] { "pt", "ao", "mz" })
        {
            var items = ReadArray(Path.Combine(culture, "preferenceparameters.json"));
            Assert.Equal(string.Empty, ValueOf(items, "SEND_MAIL_SMTP_USERNAME"));
            Assert.Equal(string.Empty, ValueOf(items, "SEND_MAIL_SMTP_PASSWORD"));
        }
    }

    [Fact]
    public void PreferenceParameters_Pt_HasSingleTransportDocumentsToken()
    {
        var items = ReadArray(Path.Combine("pt", "preferenceparameters.json"));
        var matches = items.Where(item => item.GetProperty("Token").GetString() == "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS").ToList();
        Assert.Single(matches);
    }

    [Fact]
    public void PreferenceParameters_Pt_MailTemplates_UseEuropeanPortuguese()
    {
        var items = ReadArray(Path.Combine("pt", "preferenceparameters.json"));
        Assert.Equal("Envio de documentos eletrónicos", ValueOf(items, "SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT"));
        var body = ValueOf(items, "SEND_MAIL_FINANCE_DOCUMENTS_BODY");
        Assert.Contains("eletrónico", body, StringComparison.Ordinal);
        Assert.Contains("Faturação", body, StringComparison.Ordinal);
        Assert.DoesNotContain("electrónico", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Facturação", body, StringComparison.Ordinal);
    }

    [Fact]
    public void ArticleClasses_Imposto_UsesExcetoSpelling()
    {
        var items = ReadArray("articleclasses.json");
        var imposto = items.Single(item => item.GetProperty("Acronym").GetString() == "I");
        Assert.Equal("Imposto (exceto IVA e IS) ou Encargo Parafiscal", imposto.GetProperty("Designation").GetString());
    }

    [Fact]
    public void PaymentConditions_Lpd_UsesExpandedDesignation()
    {
        var items = ReadArray("paymentconditions.json");
        var lpd = items.Single(item => item.GetProperty("Acronym").GetString() == "LPD");
        Assert.Equal("c07a5c41-37b2-4bf3-9469-5faf1799a89f", lpd.GetProperty("Id").GetString());
        Assert.Equal("Letra para desconto", lpd.GetProperty("Designation").GetString());
    }

    [Fact]
    public void DocumentTypes_Pt_Trv_UsesDepositRefundDesignation()
    {
        var items = ReadArray(Path.Combine("pt", "documenttypes.json"));
        var trv = items.Single(item => item.GetProperty("Acronym").GetString() == "TRV");
        Assert.Equal("020f94b0-8006-4465-aad6-aa478df20f42", trv.GetProperty("Id").GetString());
        Assert.Equal("Talão de reembolso de depósito", trv.GetProperty("Designation").GetString());
    }

    [Theory]
    [InlineData("pt")]
    [InlineData("ao")]
    [InlineData("mz")]
    public void Customers_Interno_Supplier_IsNotDeleted(string culture)
    {
        var items = ReadArray(Path.Combine(culture, "customers.json"));
        var interno = items.Single(item => item.GetProperty("Name").GetString() == "Interno");
        Assert.True(interno.GetProperty("Supplier").GetBoolean());
        Assert.False(interno.GetProperty("IsDeleted").GetBoolean());
    }

    [Theory]
    [InlineData("pt")]
    [InlineData("ao")]
    [InlineData("mz")]
    public void Customers_HasSingleFinalConsumer(string culture)
    {
        var items = ReadArray(Path.Combine(culture, "customers.json"));
        var consumers = items.Where(item => item.GetProperty("Name").GetString() == "Consumidor Final").ToList();
        Assert.Single(consumers);
        Assert.Equal("0cf40622-578b-417d-b50f-e945fefb5d68", consumers[0].GetProperty("Id").GetString());
    }

    [Theory]
    [InlineData("cafe")]
    [InlineData("restaurant")]
    public void DemoArticles_UseCorrectedPortugueseDemoNames(string module)
    {
        var articles = ReadArray(Path.Combine("modules", module, "articles.json"));
        var designations = articles.Select(item => item.GetProperty("Designation").GetString()!).ToHashSet(StringComparer.Ordinal);

        Assert.Contains("Toucinho do Céu", designations);
        Assert.Contains("Mousse de Chocolate com Chantilly", designations);
        Assert.Contains("Tarte de Amêndoa", designations);
        Assert.Contains("Meia de leite", designations);
        Assert.Contains("Red Bull", designations);
        Assert.Contains("Super Bock Abadia", designations);
        Assert.Contains("Água Luso 33cl", designations);
        Assert.Contains("Ponte de Lima", designations);
        Assert.Contains("Casal Garcia", designations);
        Assert.Contains("Bola de Berlim", designations);
        if (module == "restaurant")
        {
            Assert.Contains("Vodka Smirnoff", designations);
            Assert.DoesNotContain("Vodka smirnoff", designations);
        }

        Assert.Contains(designations, name => name.StartsWith("Hambúrguer", StringComparison.Ordinal));
        Assert.DoesNotContain("Red bull", designations);
        Assert.DoesNotContain("Super bock abadia", designations);
        Assert.DoesNotContain("Outras Caf", designations);
        Assert.DoesNotContain("YOUR-EMAIL@gmail.com", designations);
    }

    [Theory]
    [InlineData("cafe")]
    [InlineData("restaurant")]
    public void DemoSubfamilies_ExpandOutrasCafAbbreviation(string module)
    {
        var items = ReadArray(Path.Combine("modules", module, "articlesubfamilies.json"));
        var designations = items.Select(item => item.GetProperty("Designation").GetString()!).ToList();
        Assert.Contains("Outros de cafetaria", designations);
        Assert.DoesNotContain("Outras Caf", designations);
        Assert.DoesNotContain("Outras cafetaria", designations);
    }

    private static string ValueOf(JsonElement[] items, string token)
        => items.Single(item => item.GetProperty("Token").GetString() == token).GetProperty("Value").GetString() ?? string.Empty;

    private static JsonElement[] ReadArray(string relativePath)
    {
        var path = Path.Combine(SeedTestPaths.FindRepoRoot(), "db_seeds", relativePath);
        Assert.True(File.Exists(path), $"Missing seed file: {path}");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.EnumerateArray().Select(item => item.Clone()).ToArray();
    }
}
