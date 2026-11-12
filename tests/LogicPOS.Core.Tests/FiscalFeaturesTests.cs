using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.ValueObjects;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class FiscalFeaturesTests
{
    [Theory]
    [InlineData("PT", false, true)]
    [InlineData("AO", true, false)]
    [InlineData("MZ", false, false)]
    public void Agt_is_angola_only_and_at_communication_is_portugal_only(string code, bool agt, bool at)
    {
        var country = FiscalCountry.Parse(code);

        Assert.Equal(agt, FiscalFeatures.ShowsAgt(country));
        Assert.Equal(at, FiscalFeatures.ShowsAtCommunication(country));
    }

    [Theory]
    [InlineData("AGT_FE_RSA_PRIVATE_KEY", "PT", false)]
    [InlineData("AGT_FE_RSA_PRIVATE_KEY", "MZ", false)]
    [InlineData("AGT_FE_RSA_PRIVATE_KEY", "AO", true)]
    [InlineData("SERVICE_AGT_FE_ENABLED", "AO", true)]
    [InlineData("SERVICE_AGT_FE_ENABLED", "PT", false)]
    [InlineData("SERVICE_AGT_SEND_DOCUMENTS", "MZ", false)]
    [InlineData("AGT_FE_QRCODE_LOGO", "MZ", false)]
    [InlineData("AGT_FE_QRCODE_LOGO", "AO", true)]
    [InlineData("SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD", "PT", true)]
    [InlineData("SERVICE_AT_SEND_DOCUMENTS", "AO", false)]
    [InlineData("SERVICE_AT_SEND_DOCUMENTS", "MZ", false)]
    [InlineData("PATH_SAFTPT", "AO", true)]
    [InlineData("PATH_SAFTPT", "MZ", true)]
    [InlineData("COMPANY_NAME", "AO", true)]
    public void Preference_visibility_follows_fiscal_country(string token, string code, bool visible)
        => Assert.Equal(visible, FiscalFeatures.IsPreferenceVisible(token, FiscalCountry.Parse(code)));

    [Theory]
    [InlineData("ATDocCodeValidationSeries", "PT", true)]
    [InlineData("ATDocCodeValidationSeries", "AO", false)]
    [InlineData("WsAtDocument", "MZ", false)]
    [InlineData("SaftDocumentType", "AO", true)]
    [InlineData("SaftAuditFile", "MZ", true)]
    public void Entity_field_visibility_follows_fiscal_country(string property, string code, bool visible)
        => Assert.Equal(visible, FiscalFeatures.IsEntityFieldVisible(property, FiscalCountry.Parse(code)));

    [Theory]
    [InlineData("PT", true, false)]
    [InlineData("AO", false, true)]
    [InlineData("MZ", false, false)]
    public async Task Database_shows_only_its_country_at_and_agt_features(string code, bool at, bool agt)
    {
        var root = Directory.CreateTempSubdirectory($"logicpos-fiscal-{code}-");
        try
        {
            ConfigureApp(root, code);
            var services = AppComposition.Services!;
            var listing = services.GetRequiredService<IBackOfficeListingService>();

            var draft = await services.GetRequiredService<ICompanySetupService>().LoadAsync();
            Assert.Equal(agt, draft.Fields.Any(field => field.Token.StartsWith("AGT_FE", StringComparison.OrdinalIgnoreCase)));

            var system = await listing.QueryAsync("Parâmetros de Sistema");
            var company = await listing.QueryAsync("Parâmetros da Empresa");
            Assert.Equal(at, system.Rows.Any(row => row.Values["Token"].StartsWith("SERVICE_AT_", StringComparison.Ordinal)));
            Assert.Contains(system.Rows, row => row.Values["Token"] == "PATH_SAFTPT");
            Assert.Equal(agt, company.Rows.Any(row => row.Values["Token"].StartsWith("AGT_FE", StringComparison.Ordinal)));

            var seriesFields = await listing.LoadFieldsAsync("Séries", null);
            var series = await listing.QueryAsync("Séries");
            Assert.Equal(at, seriesFields.Any(field => field.Key == "ATDocCodeValidationSeries"));
            Assert.Equal(at, series.Actions.Any(action => action.Key == "register-at"));
            Assert.Equal(agt, series.Actions.Any(action => action.Key == "request-agt"));

            var wizard = services.GetRequiredService<IFiscalYearWizard>();
            var year = await wizard.LoadAsync();
            var created = await wizard.CreateAsync(new FiscalYearSetupRequest
            {
                Designation = year.Designation,
                Year = year.Year,
                Acronym = year.Acronym,
                CreateSeries = true,
                DocumentTypeIds = year.DocumentTypes.Select(type => type.Id).ToList()
            });
            Assert.True(created.Ok, created.Message);

            var issues = await services.GetRequiredService<IStartupReadinessService>().GetIssuesAsync();
            Assert.DoesNotContain(issues, issue => issue.Key == "series");
            Assert.Equal(at, issues.Any(issue => issue.Key == "series-at"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static void ConfigureApp(DirectoryInfo root, string country)
    {
        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        File.WriteAllText(
            Path.Combine(root.FullName, "appsettings.json"),
            $$"""
            {
              "Country": "{{country}}",
              "Culture": "pt-PT",
              "DatabaseSettings": {
                "DatabaseType": "Sqlite",
                "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                "UseCloud": false,
                "Module": "restaurant"
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        Assert.NotNull(AppComposition.Services);
    }

    private static void Cleanup(DirectoryInfo root)
    {
        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        SqliteConnection.ClearAllPools();
        AppLogging.CloseAndFlush();

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                if (root.Exists)
                {
                    root.Delete(recursive: true);
                }

                return;
            }
            catch (IOException) when (attempt < 5)
            {
                Thread.Sleep(200 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < 5)
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }
}
