using LogicPOS.Core.Updates;

namespace LogicPOS.Core.Tests.Updates;

public sealed class UpdatePromptDecisionTests
{
    [Fact]
    public void Asks_only_when_the_published_release_is_newer_and_names_it()
    {
        var newer = UpdatePromptDecision.For("2.0.40", "2.0.35");
        var same = UpdatePromptDecision.For("2.0.35", "2.0.35");
        var unknown = UpdatePromptDecision.For(null, "2.0.35");

        Assert.True(newer.Ask);
        Assert.Contains("2.0.40", newer.Question);
        Assert.StartsWith("Descarregar a atualização", newer.Question);
        Assert.False(same.Ask);
        Assert.Equal(string.Empty, same.Question);
        Assert.False(unknown.Ask);
        Assert.Equal(string.Empty, unknown.Question);
        Assert.True(ApplicationVersionComparer.IsNewer("2.0.40", "2.0.35"));
    }
}
