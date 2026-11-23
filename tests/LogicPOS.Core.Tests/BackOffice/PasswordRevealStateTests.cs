using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class PasswordRevealStateTests
{
    [Fact]
    public void Starts_hidden_and_tips_are_portuguese()
    {
        Assert.True(PasswordRevealState.IsHidden(PasswordRevealState.Mask));
        Assert.Equal("Mostrar palavra-passe", PasswordRevealState.Tip(currentlyHidden: true));
        Assert.Equal("Ocultar palavra-passe", PasswordRevealState.Tip(currentlyHidden: false));
    }

    [Fact]
    public void Toggle_alternates_mask_without_exposing_secret_values()
    {
        var hidden = true;
        var next = PasswordRevealState.NextPasswordChar(hidden);
        Assert.Equal('\0', next);
        Assert.False(PasswordRevealState.IsHidden(next));

        next = PasswordRevealState.NextPasswordChar(currentlyHidden: false);
        Assert.Equal(PasswordRevealState.Mask, next);
        Assert.True(PasswordRevealState.IsHidden(next));
    }
}
