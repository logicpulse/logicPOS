using System.Globalization;
using LogicPOS.Core;

namespace LogicPOS.Core.Tests;

[Collection(UiCultureCollection.Name)]
public sealed class UiCultureTests
{
    [Fact]
    public void Read_uses_cache_without_touching_disk_after_SetCachedForTests()
    {
        var previous = Capture();
        try
        {
            UiCulture.SetCachedForTests("en-US");
            Assert.Equal("en-US", UiCulture.Read(null));
            UiCulture.SetCachedForTests("pt-PT");
            Assert.Equal("pt-PT", UiCulture.Read(null));
        }
        finally
        {
            Restore(previous);
        }
    }

    [Fact]
    public void Apply_updates_cache_and_CurrentUICulture()
    {
        var previous = Capture();
        try
        {
            UiCulture.Apply("en-GB");
            Assert.Equal("en-GB", UiCulture.Read("pt-PT"));
            Assert.Equal("en-GB", CultureInfo.CurrentUICulture.Name);
        }
        finally
        {
            Restore(previous);
        }
    }

    [Fact]
    public void Apply_falls_back_to_pt_PT_for_invalid_name()
    {
        var previous = Capture();
        try
        {
            UiCulture.Apply("not-a-real-culture-xyz");
            Assert.Equal("pt-PT", UiCulture.Read(null));
        }
        finally
        {
            Restore(previous);
        }
    }

    private static (string Cached, CultureInfo Culture, CultureInfo Ui) Capture()
        => (UiCulture.Read(null), CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

    private static void Restore((string Cached, CultureInfo Culture, CultureInfo Ui) previous)
    {
        try
        {
            UiCulture.Apply(previous.Cached);
        }
        catch
        {
            UiCulture.SetCachedForTests(previous.Cached);
        }

        CultureInfo.CurrentCulture = previous.Culture;
        CultureInfo.CurrentUICulture = previous.Ui;
    }
}
