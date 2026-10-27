using Avalonia.Controls;

namespace LogicPOS.App.Views;

/// <summary>
/// Desktop-only shell: Avalonia Android cannot create Window, so POS UI lives in PosWindow (UserControl).
/// </summary>
public partial class PosShellWindow : Window
{
    public PosShellWindow(PosWindow pos)
    {
        InitializeComponent();
        Host.Children.Add(pos);
        Opened += async (_, _) => await pos.ActivateHostedAsync();
        pos.LoggedOut += (_, _) => Close();
    }
}
