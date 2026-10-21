using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.Finance;

public sealed class TransportAndExemptionTests
{
    private static readonly PosCountryPostalRule[] Countries =
    [
        new() { Designation = "Portugal", Code2 = "PT", Code3 = "PRT", ZipCodeRegex = @"^\d{4}-\d{3}?$" },
        new() { Designation = "Espanha", Code2 = "ES", Code3 = "ESP", ZipCodeRegex = @"^\d{5}?$" }
    ];

    [Fact]
    public void ValidateTransport_accepts_spanish_postal_code_from_country_table()
    {
        var origin = Ship("Portugal", "3080-245", hoursFromNow: 2);
        var destination = Ship("Espanha", "42162", hoursFromNow: 5);

        Assert.Null(PosDocumentRules.ValidateTransport(destination, origin, Countries));
    }

    [Fact]
    public void ValidateTransport_rejects_postal_code_that_breaks_the_country_regex()
    {
        var origin = Ship("PT", "3080245", hoursFromNow: 2);
        var destination = Ship("ES", "42162", hoursFromNow: 5);

        var error = PosDocumentRules.ValidateTransport(destination, origin, Countries);

        Assert.NotNull(error);
        Assert.Contains("Portugal", error);
    }

    [Fact]
    public void ValidateTransport_uses_the_regex_stored_on_the_country()
    {
        PosCountryPostalRule[] custom =
        [
            new() { Designation = "Portugal", Code2 = "PT", Code3 = "PRT", ZipCodeRegex = @"^\d{4}-\d{3}?$" },
            new() { Designation = "Espanha", Code2 = "ES", Code3 = "ESP", ZipCodeRegex = @"^ES\d{5}$" }
        ];
        var origin = Ship("Portugal", "3080-245", hoursFromNow: 2);
        var destination = Ship("Espanha", "42162", hoursFromNow: 5);

        var error = PosDocumentRules.ValidateTransport(destination, origin, custom);

        Assert.NotNull(error);
        Assert.Contains("Espanha", error);
    }

    [Fact]
    public void ToAddress_sends_the_country_iso_code()
    {
        var destination = Ship("Espanha", "42162", hoursFromNow: 5);

        var address = PosDocumentRules.ToAddress(destination, Countries);

        Assert.Equal("ES", address.Country);
    }

    [Fact]
    public void ValidateTransport_allows_unknown_district()
    {
        var origin = Ship("Portugal", "3080-245", hoursFromNow: 2, region: "Desconhecido");
        var destination = Ship("Espanha", "42162", hoursFromNow: 5, region: "Desconhecido");

        Assert.Null(PosDocumentRules.ValidateTransport(destination, origin, Countries));
    }

    [Fact]
    public void MatchVatExemptionId_finds_reason_by_acronym_or_designation()
    {
        var id = Guid.Parse("2fee11a9-8125-46d4-91a6-0309784d8db7");
        var reasons = new[]
        {
            new PosLookupItem(id, "M06 - Art.º 15.º CIVA", "260")
        };

        Assert.Equal(id, PosDocumentRules.MatchVatExemptionId(reasons, "M06", null));
        Assert.Equal(id, PosDocumentRules.MatchVatExemptionId(reasons, "260", null));
        Assert.Equal(id, PosDocumentRules.MatchVatExemptionId(reasons, null, "Art.º 15.º CIVA"));
        Assert.Null(PosDocumentRules.MatchVatExemptionId(reasons, null, null));
    }

    private static PosShipAddress Ship(string country, string postal, int hoursFromNow, string region = "Coimbra")
    {
        return new PosShipAddress
        {
            Address = "Rua 1",
            Region = region,
            PostalCode = postal,
            City = "Cidade",
            Country = country,
            When = DateTime.Now.AddHours(hoursFromNow).ToString("yyyy-MM-ddTHH:mm:ss")
        };
    }
}
