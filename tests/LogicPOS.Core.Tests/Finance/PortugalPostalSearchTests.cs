using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Finance.Postal;

namespace LogicPOS.Core.Tests.Finance;

public sealed class PortugalPostalSearchTests
{
    private const string Cp7Json = """
        {
          "CP": "1000-001",
          "Localidade": "Lisboa",
          "Concelho": "Lisboa",
          "partes": [{ "Artéria": "Rua dos Açores" }]
        }
        """;

    private const string Cp4Json = """
        {
          "CP4": "2495",
          "Localidade": "Fátima",
          "Concelho": "Ourém",
          "CP3": ["300", "301"]
        }
        """;

    [Fact]
    public void SupportsSuggestion_is_only_portugal()
    {
        Assert.True(PortuguesePostalFormats.SupportsSuggestion("PT"));
        Assert.False(PortuguesePostalFormats.SupportsSuggestion("AO"));
        Assert.False(PortuguesePostalFormats.SupportsSuggestion("MZ"));
        Assert.False(PortuguesePostalFormats.SupportsSuggestion("ES"));
    }

    [Fact]
    public async Task Complete_code_fills_locality_and_city_without_autocomplete_on_prefix()
    {
        var search = CreateSearch(new Dictionary<string, string?>
        {
            ["1000-001"] = Cp7Json,
            ["2495"] = Cp4Json
        });

        var complete = await search.SearchAsync("1000-001", PostalSearchTrigger.AutoComplete);
        Assert.Single(complete);
        Assert.Equal("Lisboa", complete[0].Locality);
        Assert.Equal("Lisboa", complete[0].City);

        var prefix = await search.SearchAsync("2495", PostalSearchTrigger.AutoComplete);
        Assert.Empty(prefix);
    }

    [Fact]
    public async Task Parser_includes_street_and_selection_omits_street_when_ambiguous_on_autofill()
    {
        var search = CreateSearch(new Dictionary<string, string?> { ["1000-001"] = Cp7Json });
        var matches = await search.SearchAsync("1000-001", PostalSearchTrigger.AutoComplete);
        Assert.Equal("Rua dos Açores", matches[0].Street);

        var ambiguous = new[]
        {
            new PostalLocalityMatch { ZipCode = "1000-001", Locality = "Lisboa", City = "Lisboa", Street = "Rua A" },
            new PostalLocalityMatch { ZipCode = "1000-001", Locality = "Lisboa", City = "Lisboa", Street = "Rua B" }
        };
        var chosen = PostalLocalitySelection.ChooseForAutoFill(ambiguous, "1000-001", explicitRequest: false);
        Assert.NotNull(chosen);
        Assert.Equal(string.Empty, chosen!.Street);
    }

    [Fact]
    public async Task Prefix_list_only_when_explicitly_requested()
    {
        var search = CreateSearch(new Dictionary<string, string?> { ["2495"] = Cp4Json });
        var matches = await search.SearchAsync("2495", PostalSearchTrigger.Explicit);
        Assert.Equal(2, matches.Count);
        Assert.Equal("2495-300", matches[0].ZipCode);
        Assert.True(PostalLocalitySelection.ShouldOfferPicker(matches, "2495", explicitRequest: true));
    }

    [Fact]
    public async Task Unknown_or_offline_returns_empty_without_error()
    {
        var search = CreateSearch(new Dictionary<string, string?>(), failRequests: true);
        var unknown = await search.SearchAsync("9999-999", PostalSearchTrigger.AutoComplete);
        Assert.Empty(unknown);

        var offline = await search.SearchAsync("1000-001", PostalSearchTrigger.AutoComplete);
        Assert.Empty(offline);
    }

    [Fact]
    public void Chooser_keeps_street_on_explicit_complete_request()
    {
        var matches = new[]
        {
            new PostalLocalityMatch
            {
                ZipCode = "1000-001",
                Locality = "Lisboa",
                City = "Lisboa",
                Street = "Rua dos Açores"
            }
        };

        var chosen = PostalLocalitySelection.ChooseForAutoFill(matches, "1000-001", explicitRequest: true);
        Assert.Equal("Rua dos Açores", chosen?.Street);
    }

    private static PortugalPostalSearch CreateSearch(
        Dictionary<string, string?> responses,
        bool failRequests = false)
        => new(new FakePortugalGeoApiClient(responses, failRequests));

    private sealed class FakePortugalGeoApiClient(Dictionary<string, string?> responses, bool failRequests) : IPortugalGeoApiClient
    {
        public Task<string?> GetPostalCodeJsonAsync(string cp, CancellationToken cancellationToken = default)
        {
            if (failRequests)
            {
                throw new HttpRequestException("offline");
            }

            return Task.FromResult(responses.TryGetValue(cp, out var json) ? json : null);
        }
    }
}