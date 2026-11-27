using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Core.Tests.Domain;

public class PortugueseNifTests
{
    [Theory]
    [InlineData("100000002")]
    [InlineData("200000004")]
    [InlineData("300000006")]
    [InlineData("500000000")]
    [InlineData("600000001")]
    [InlineData("450000001")]
    [InlineData("700000003")]
    [InlineData("710000006")]
    [InlineData("720000009")]
    [InlineData("740000004")]
    [InlineData("750000007")]
    [InlineData("770000002")]
    [InlineData("780000005")]
    [InlineData("790000008")]
    [InlineData("800000005")]
    [InlineData("900000007")]
    [InlineData("910000000")]
    [InlineData("980000009")]
    [InlineData("990000001")]
    [InlineData("123456789")]
    [InlineData("999999990")]
    public void IsValid_accepts_known_valid_numbers(string nif)
    {
        Assert.True(PortugueseNif.IsValid(nif));
        Assert.True(PortugueseNif.TryValidate(nif, out var result));
        Assert.True(result.IsValid);
        Assert.Equal(PortugueseNifInvalidReason.None, result.Reason);
        Assert.Equal(nif, result.Normalized);
    }

    [Theory]
    [InlineData(" 123456789 ")]
    [InlineData("PT123456789")]
    [InlineData("pt999999990")]
    public void IsValid_trims_spaces_and_optional_pt_prefix(string nif)
    {
        Assert.True(PortugueseNif.IsValid(nif));
    }

    [Theory]
    [InlineData(null, PortugueseNifInvalidReason.Empty)]
    [InlineData("", PortugueseNifInvalidReason.Empty)]
    [InlineData("   ", PortugueseNifInvalidReason.Empty)]
    [InlineData("12345678", PortugueseNifInvalidReason.Length)]
    [InlineData("1234567890", PortugueseNifInvalidReason.Length)]
    [InlineData("12345678A", PortugueseNifInvalidReason.NonDigits)]
    [InlineData("12-456789", PortugueseNifInvalidReason.NonDigits)]
    [InlineData("400000008", PortugueseNifInvalidReason.Prefix)]
    [InlineData("411111116", PortugueseNifInvalidReason.Prefix)]
    [InlineData("123456780", PortugueseNifInvalidReason.CheckDigit)]
    public void TryValidate_reports_expected_reason(string? nif, PortugueseNifInvalidReason reason)
    {
        Assert.False(PortugueseNif.TryValidate(nif, out var result));
        Assert.False(result.IsValid);
        Assert.Equal(reason, result.Reason);
        Assert.Null(result.Normalized);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("PT", true)]
    [InlineData("pt", true)]
    [InlineData("ES", false)]
    [InlineData("es", false)]
    [InlineData("AO", false)]
    public void AppliesToCountry_defaults_empty_to_portugal(string? code2, bool expected)
    {
        Assert.Equal(expected, PortugueseNif.AppliesToCountry(code2));
    }
}
