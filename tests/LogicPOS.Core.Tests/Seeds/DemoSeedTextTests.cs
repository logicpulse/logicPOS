using System.Text.Json;

namespace LogicPOS.Core.Tests.Seeds;

public sealed class DemoSeedTextTests
{
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

    [Theory]
    [InlineData("cafe")]
    [InlineData("restaurant")]
    public void DemoArticles_UseCorrectedPortugueseDemoNames(string module)
    {
        var articles = ReadArray(Path.Combine("modules", module, "articles.json"));
        var designations = articles.Select(item => item.GetProperty("Designation").GetString()!).ToHashSet(StringComparer.Ordinal);

        Assert.Contains("Toucinho do Céu", designations);
        Assert.Contains("Mousse de chocolate com chantilly", designations);
        Assert.Contains("Tarte de Amêndoa", designations);
        Assert.Contains("Meia de leite", designations);
        Assert.Contains("Red Bull", designations);
        Assert.Contains("Super Bock Abadia", designations);
        Assert.Contains("Água Luso 33cl", designations);
        Assert.Contains("Ponte de Lima", designations);
        Assert.Contains("Casal Garcia", designations);
        Assert.Contains("Bola de Berlim", designations);
        Assert.Contains("Grão Vasco", designations);
        Assert.Contains("Mateus Rosé", designations);
        Assert.Contains("Conde de Villar", designations);
        Assert.Contains("Porca de Murça", designations);
        Assert.Contains("Dona Maria", designations);
        Assert.Contains("Tapada dos Monges", designations);
        if (module == "restaurant")
        {
            Assert.Contains("Vodka Smirnoff", designations);
            Assert.DoesNotContain("Vodka smirnoff", designations);
            Assert.Contains("Cutty Sark", designations);
            Assert.Contains("Brandy Macieira", designations);
            Assert.Contains("Gin N1 The London", designations);
            Assert.Contains("Bacardi Pina Colada", designations);
            Assert.Contains("Bacalhau à Brás", designations);
            Assert.Contains("Bacalhau à Gomes Sá", designations);
            Assert.Contains("Terras do Demo Bruto", designations);
            Assert.Contains("Montanha Real", designations);
            Assert.Contains("Asti Gancia Doce", designations);
        }

        Assert.Contains(designations, name => name.StartsWith("Hambúrguer", StringComparison.Ordinal));
        Assert.DoesNotContain("Red bull", designations);
        Assert.DoesNotContain("Super bock abadia", designations);
        Assert.DoesNotContain("Grão vasco", designations);
        Assert.DoesNotContain("Mateus rosé", designations);
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
