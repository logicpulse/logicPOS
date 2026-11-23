using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.Tests.Finance;

public class DocumentCalculateTotalsTests
{
    [Fact]
    public void CalculateTotals_applies_stored_precision_to_sum()
    {
        var document = new Document
        {
            Type = "FT",
            TotalNet = 2.5m,
            TotalTax = 2.49999999999999m
        };

        document.CalculateTotals();

        Assert.Equal(5.000000m, document.TotalFinal);
        Assert.Equal((2.5m + 2.49999999999999m).ToStoredPrecision(), document.TotalFinal);
    }

    [Fact]
    public void CalculateTotals_noise_case_matches_to_stored_precision()
    {
        // Classic double/decimal noise that must not remain on TotalFinal before hash/persist.
        var document = new Document
        {
            Type = "FT",
            TotalNet = 2.5m,
            TotalTax = 2.49999999999999m
        };

        var rawSum = document.TotalNet + document.TotalTax;
        Assert.NotEqual(5.000000m, rawSum);

        document.CalculateTotals();
        Assert.Equal(5.000000m, document.TotalFinal);
    }

    [Fact]
    public void CalculateTotals_clean_six_decimal_inputs_are_stable()
    {
        var document = new Document
        {
            Type = "FT",
            TotalNet = 10.123456m,
            TotalTax = 2.345678m
        };

        document.CalculateTotals();
        Assert.Equal(12.469134m, document.TotalFinal);
    }

    [Fact]
    public void Stored_precision_total_rounds_to_same_two_decimals_as_raw_noise_sum()
    {
        // SAF-T / document hash money text uses Round() / To2DecimalPlacesString (see RoundingExtension).
        // Clamping TotalFinal to 6dp must not change the 2dp string used for signing/export.
        var net = 2.5m;
        var tax = 2.49999999999999m;
        var raw = net + tax;
        var stored = raw.ToStoredPrecision();

        Assert.Equal(raw.Round(), stored.Round());
        Assert.Equal(raw.To2DecimalPlacesString(), stored.To2DecimalPlacesString());
        Assert.Equal("5.00", stored.To2DecimalPlacesString());
    }

    [Fact]
    public void Already_issued_hash_is_not_recomputed_by_calculate_totals()
    {
        // Hash is assigned once in DocumentCreator.AssignHashAsync at create time from TotalFinal.
        // Reprint/read paths use the persisted Hash; CalculateTotals does not touch Hash.
        var document = new Document
        {
            Type = "FT",
            TotalNet = 10m,
            TotalTax = 2.3m,
            Hash = "PERSISTED-HASH-VALUE"
        };

        document.CalculateTotals();

        Assert.Equal(12.3m, document.TotalFinal);
        Assert.Equal("PERSISTED-HASH-VALUE", document.Hash);
    }
}
