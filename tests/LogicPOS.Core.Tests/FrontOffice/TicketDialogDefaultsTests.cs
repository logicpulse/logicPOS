using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class TicketDialogDefaultsTests
{
    [Fact]
    public void Cancel_order_confirm_defaults_to_nao()
    {
        Assert.True(TicketDialogDefaults.PreferCancelAsDefault(showCancel: true, confirmText: "Sim"));
    }

    [Fact]
    public void Ok_only_notice_keeps_confirm_default()
    {
        Assert.False(TicketDialogDefaults.PreferCancelAsDefault(showCancel: false, confirmText: "Ok"));
    }

    [Fact]
    public void Keypad_ok_cancel_keeps_confirm_default()
    {
        Assert.False(TicketDialogDefaults.PreferCancelAsDefault(showCancel: true, confirmText: "Ok"));
    }
}
