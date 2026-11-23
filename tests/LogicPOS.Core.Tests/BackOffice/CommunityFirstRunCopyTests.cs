using LogicPOS.Core;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class CommunityFirstRunCopyTests
{
    [Fact]
    public void Public_edition_keeps_uncertified_community_copy()
    {
        Assert.Contains("faturação local sem certificação AT integrada", CommunityFirstRunCopy.Intro(false));
        Assert.Contains("sem certificação AT integrada", CommunityFirstRunCopy.Feature(false));
    }

    [Fact]
    public void LogicPulse_community_states_certification_and_keeps_business_and_cloud_certified()
    {
        var intro = CommunityFirstRunCopy.Intro(true);
        var feature = CommunityFirstRunCopy.Feature(true);

        Assert.Contains("faturação local com certificação AT", intro);
        Assert.Contains("Business e Cloud acrescentam certificação LogicPulse", intro);
        Assert.DoesNotContain("sem certificação AT integrada", intro);
        Assert.DoesNotContain("sem certificação AT integrada", feature);
        Assert.Contains("com certificação AT", feature);
    }
}
