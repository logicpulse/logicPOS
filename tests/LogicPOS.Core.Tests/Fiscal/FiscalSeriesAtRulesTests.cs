using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Core.Tests.Fiscal;

public sealed class FiscalSeriesAtRulesTests
{
    [Theory]
    [InlineData("FT", true)]
    [InlineData("FS", true)]
    [InlineData("FR", true)]
    [InlineData("NC", true)]
    [InlineData("RC", true)]
    [InlineData("GR", true)]
    [InlineData("OR", false)]
    [InlineData("CC", false)]
    public void Portugal_with_fiscal_module_gates_listed_document_types(string acronym, bool gated)
    {
        Assert.Equal(gated, FiscalSeriesAtRules.RequiresAtSeriesCode(acronym, FiscalCountry.Portugal, fiscalModuleAvailable: true));
    }

    [Theory]
    [InlineData("FS")]
    [InlineData("FT")]
    public void Without_fiscal_module_portugal_does_not_gate(string acronym)
    {
        Assert.False(FiscalSeriesAtRules.RequiresAtSeriesCode(acronym, FiscalCountry.Portugal, fiscalModuleAvailable: false));
    }

    [Theory]
    [InlineData("FS")]
    [InlineData("FT")]
    public void Angola_never_gates_on_at_series(string acronym)
    {
        Assert.False(FiscalSeriesAtRules.RequiresAtSeriesCode(acronym, FiscalCountry.Angola, fiscalModuleAvailable: true));
    }

    [Fact]
    public void Missing_at_code_returns_communication_refusal()
    {
        var message = FiscalSeriesAtRules.RefusalMessageIfMissing(
            "FS",
            atDocCodeValidationSeries: null,
            FiscalCountry.Portugal,
            fiscalModuleAvailable: true);
        Assert.Contains("não foi comunicada", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Communicated_series_allows_issue()
    {
        var message = FiscalSeriesAtRules.RefusalMessageIfMissing(
            "FS",
            "ABC1234X",
            FiscalCountry.Portugal,
            fiscalModuleAvailable: true);
        Assert.Null(message);
    }

    [Fact]
    public void Without_fiscal_module_missing_code_is_not_refused()
    {
        var message = FiscalSeriesAtRules.RefusalMessageIfMissing(
            "FS",
            null,
            FiscalCountry.Portugal,
            fiscalModuleAvailable: false);
        Assert.Null(message);
    }
}