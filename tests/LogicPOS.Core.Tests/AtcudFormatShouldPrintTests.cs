using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests;

public sealed class AtcudFormatShouldPrintTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("0-1")]
    public void ShouldPrint_returns_false_for_missing_or_placeholder_atcud(string? atcud)
        => Assert.False(AtcudFormat.ShouldPrint(atcud));

    [Fact]
    public void ShouldPrint_returns_true_for_valid_atcud()
        => Assert.True(AtcudFormat.ShouldPrint("ABC1234X-1"));
}
