using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LogicPOS.App.Views;

public partial class FrontOfficeFirstSaleTip : UserControl
{
    public FrontOfficeFirstSaleTip()
    {
        InitializeComponent();
        EnglishUi.Apply(this);
    }

    public event EventHandler? Dismissed;

    private void OnGotItClick(object? sender, RoutedEventArgs e)
    {
        FrontOfficeFirstSaleTipStore.MarkSeen();
        Dismissed?.Invoke(this, EventArgs.Empty);
    }
}
