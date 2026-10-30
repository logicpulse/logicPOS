using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests;

public class PosLookupItemTests
{
    [Theory]
    [InlineData("FT", "FT 2026A864/1008", "FT 2026A864/1008")]
    [InlineData("PF", "PF 2026A864/1", "PF 2026A864/1")]
    [InlineData("ft", "FT 2026A864/1008", "FT 2026A864/1008")]
    [InlineData("FS", "2026A864/12", "FS 2026A864/12")]
    [InlineData("", "FT 2026A864/1008", "FT 2026A864/1008")]
    [InlineData("FT", "", "FT")]
    public void DocumentLabel_does_not_double_prefix_type(string type, string number, string expected)
        => Assert.Equal(expected, PosLookupItem.DocumentLabel(type, number));

    [Fact]
    public void ForDocument_label_matches_stored_number_when_it_already_has_type()
    {
        var item = PosLookupItem.ForDocument(
            Guid.NewGuid(),
            "FT",
            "FT 2026A864/1008",
            "Maquigomes",
            "123456789");

        Assert.Equal("FT 2026A864/1008", item.Label);
        Assert.Equal("FT", item.Code);
        Assert.Contains("Maquigomes", item.Detail);
    }
}
