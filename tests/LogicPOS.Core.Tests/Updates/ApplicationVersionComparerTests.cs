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
    [InlineData("1.6.33+retail", "1.6.4")]
    public void Higher_release_is_newer(string package, string installed)
        => Assert.True(ApplicationVersionComparer.IsNewer(package, installed));

    [Fact]
    public void Older_package_is_not_newer()
        => Assert.False(ApplicationVersionComparer.IsNewer("1.6.2", "1.6.33+develop"));

    [Fact]
    public void Unreadable_package_is_not_treated_as_an_update()
        => Assert.False(ApplicationVersionComparer.IsNewer("develop", "1.6.33"));

    [Theory]
    [InlineData(null, "1.6.33")]
    [InlineData("", "1.6.33")]
    [InlineData("   ", null)]
    public void Null_or_blank_package_is_not_newer(string? package, string? installed)
        => Assert.False(ApplicationVersionComparer.IsNewer(package, installed));

    [Fact]
    public void Null_installed_version_still_accepts_a_readable_package()
        => Assert.True(ApplicationVersionComparer.IsNewer("1.6.34", null));

    [Fact]
    public void Five_part_version_is_not_a_release()
    {
        Assert.Null(ApplicationVersionComparer.ParseRelease(null));
        Assert.Null(ApplicationVersionComparer.ParseRelease("1.6.33.0.1"));
        Assert.False(ApplicationVersionComparer.IsNewer("1.6.33.0.1", "1.6.33"));
        Assert.False(ApplicationVersionComparer.IsNewer(null, "1.2.3.4.5"));
    }

    [Theory]
    [InlineData("1.6.33-beta", "1.6.33")]
    [InlineData("1.6.33-beta.2", "1.6.33-beta.1")]
    [InlineData("1.6.33-rc.1", "1.6.33.0")]
    public void Pre_release_suffix_is_ignored_for_ordering(string package, string installed)
        => Assert.False(ApplicationVersionComparer.IsNewer(package, installed));

    [Fact]
    public void Higher_numeric_release_stays_newer_with_a_pre_release_suffix()
        => Assert.True(ApplicationVersionComparer.IsNewer("1.6.34-beta", "1.6.33"));

    [Fact]
    public void Display_drops_channel_and_trailing_zero()
    {
        Assert.Equal("1.6.33", ApplicationVersionComparer.Display("1.6.33+develop.649f143d"));
        Assert.Equal("1.6.33", ApplicationVersionComparer.Display("1.6.33.0"));
    }
}
