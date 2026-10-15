using LogicPOS.Core.Updates;

namespace LogicPOS.Core.Tests.Updates;

public class ApplicationVersionComparerTests
{
    [Theory]
    [InlineData("1.6.33", "1.6.33.0")]
    [InlineData("1.6.33+develop", "1.6.33.0")]
    [InlineData("1.6.33+develop", "1.6.33+retail")]
    [InlineData("1.6.33+develop.649f143d09a7e3511d51341badf3d04232e2d7ad", "1.6.33+develop")]
    [InlineData("v1.6.33", "1.6.33.0")]
    public void Same_release_is_not_newer(string package, string installed)
        => Assert.False(ApplicationVersionComparer.IsNewer(package, installed));

    [Theory]
    [InlineData("1.6.34", "1.6.33+develop")]
    [InlineData("1.6.34+retail", "1.6.33.0")]
    [InlineData("1.6.33.1", "1.6.33+develop.abc")]
    [InlineData("1.7", "1.6.33.0")]
    public void Higher_release_is_newer(string package, string installed)
        => Assert.True(ApplicationVersionComparer.IsNewer(package, installed));

    [Fact]
    public void Older_package_is_not_newer()
        => Assert.False(ApplicationVersionComparer.IsNewer("1.6.2", "1.6.33+develop"));

    [Fact]
    public void Unreadable_package_is_not_treated_as_an_update()
        => Assert.False(ApplicationVersionComparer.IsNewer("develop", "1.6.33"));

    [Fact]
    public void Display_drops_channel_and_trailing_zero()
    {
        Assert.Equal("1.6.33", ApplicationVersionComparer.Display("1.6.33+develop.649f143d"));
        Assert.Equal("1.6.33", ApplicationVersionComparer.Display("1.6.33.0"));
    }
}
