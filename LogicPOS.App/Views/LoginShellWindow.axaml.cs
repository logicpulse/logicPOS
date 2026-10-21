using Avalonia.Controls;

namespace LogicPOS.App.Views;

/// <summary>
/// Desktop-only shell: Avalonia Android cannot create Window, so login UI lives in LoginWindow (UserControl).
/// </summary>
public partial class LoginShellWindow : Window
{
    public LoginShellWindow()
    {
        InitializeComponent();
        DevEditionBanner.Attach(this);
        Opened += async (_, _) => await Login.ActivateHostedAsync();
    }
}
